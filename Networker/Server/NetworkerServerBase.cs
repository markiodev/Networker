using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.Server
{
    public abstract class NetworkerServerBase : INetworkerServer
    {
        private readonly ServerConfiguration configuration;
        protected readonly IContainerIoc container;
        public readonly INetworkerLogger Logger;
        private readonly PacketDeserializer packetDeserializer;
        private readonly Dictionary<string, Type> packetHandlers;
        private readonly PacketSerializer packetSerializer;
        private bool _isRunning = true;
        private Socket _tcpSocket;
        private UdpClient _udpClient;
        private Socket _udpSocket;

        protected NetworkerServerBase(ServerConfiguration configuration,
            INetworkerLogger logger,
            IList<INetworkerPacketHandlerModule> modules,
            IContainerIoc container)
        {
            this.configuration = configuration;
            this.Logger = logger;
            this.container = container;
            this.Connections = new List<TcpConnection>();
            this.packetSerializer = new PacketSerializer();
            this.packetDeserializer = new PacketDeserializer();
            this.packetHandlers = new Dictionary<string, Type>();

            foreach(var module in modules)
            {
                this.RegisterTypesFromModule(module);
            }

            this.container.RegisterSingleton<ITcpConnectionsProvider>(
                new TcpConnectionsProvider(this.Connections));
            this.container.RegisterSingleton(logger);

            this.RegisterPacketHandler<PingRequestPacket, PingRequestPacketHandler>();
        }

        public List<TcpConnection> Connections { get; }

        public EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        public EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }

        public void Broadcast<T>(T packet)
            where T: NetworkerPacketBase
        {
            if(!this.configuration.UseUdp)
            {
                throw new Exception("Cannot broadcast packet when UDP is not enabled.");
            }

            this._udpSocket.SendTo(this.packetSerializer.Serialize(packet),
                new IPEndPoint(IPAddress.Parse(this.configuration.IpAddresses[0]),
                    this.configuration.UdpPortRemote));
        }

        public IContainerIoc GetIocContainer()
        {
            return this.container;
        }

        public INetworkerServer RegisterPacketHandler<TPacketType, TPacketHandlerType>()
            where TPacketHandlerType: IServerPacketHandler
        {
            var handler = typeof(TPacketHandlerType);
            this.packetHandlers.Add(typeof(TPacketType).Name, handler);
            this.container.RegisterType(handler);
            return this;
        }

        public INetworkerServer RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            var module =
                (INetworkerPacketHandlerModule)Activator.CreateInstance(typeof(TPacketHandlerModule));
            this.RegisterTypesFromModule(module);
            return this;
        }

        public INetworkerServer Start()
        {
            if(this.configuration.UseTcp)
            {
                this.Logger.Trace($"Starting TCP Listener on port {this.configuration.TcpPort}.");

                this._tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                this._tcpSocket.Bind(new IPEndPoint(IPAddress.Parse(this.configuration.IpAddresses[0]),
                    this.configuration.TcpPort));

                this._tcpSocket.Listen(this.configuration.Advanced.MaxTcpConnections);

                new Thread(() =>
                           {
                               while(this._isRunning)
                               {
                                   if(this._tcpSocket.Poll(10, SelectMode.SelectRead))
                                   {
                                       var acceptedSocket = this._tcpSocket.Accept();
                                       var connection = new TcpConnection(acceptedSocket);

                                       this.Logger.Trace(
                                           $"TCP Client Connected. IP: {(connection.Socket.RemoteEndPoint as IPEndPoint).Address}");

                                       this.Connections.Add(connection);
                                       this.ClientConnected?.Invoke(this,
                                           new TcpConnectionConnectedEventArgs(connection));
                                   }
                               }
                           }).Start();

                new Thread(() =>
                           {
                               while(this._isRunning)
                               {
                                   foreach(var connection in this.Connections.ToList())
                                   {
                                       if(!connection.Socket.Connected)
                                       {
                                           this.Logger.Trace(
                                               $"TCP Client Disconnected. IP: {(connection.Socket.RemoteEndPoint as IPEndPoint).Address}");

                                           this.Connections.Remove(connection);
                                           this.ClientDisconnected?.Invoke(this,
                                               new TcpConnectionDisconnectedEventArgs(connection));
                                           continue;
                                       }

                                       if(connection.Socket.Poll(10, SelectMode.SelectRead))
                                       {
                                           var packets =
                                               this.packetDeserializer
                                                   .GetPacketsFromSocket(connection.Socket);

                                           foreach(var packet in packets)
                                           {
                                               this.HandlePacket(connection, packet.Item1, packet.Item2);
                                           }
                                       }
                                   }
                               }
                           }).Start();
            }

            if(this.configuration.UseUdp)
            {
                this.Logger.Trace($"Starting UDP Listener on port {this.configuration.UdpPortLocal}.");
                this.Logger.Trace($"Starting UDP Sender on port {this.configuration.UdpPortRemote}.");

                this._udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this._udpClient = new UdpClient(this.configuration.UdpPortLocal);

                new Thread(() =>
                           {
                               while(this._isRunning)
                               {
                                   var result = this._udpClient.ReceiveAsync()
                                                    .Result;

                                   var packets = this.packetDeserializer.GetPacketsFromUdp(result);

                                   foreach(var packet in packets)
                                   {
                                       this.HandlePacket(new UdpConnection(this._udpSocket, result),
                                           packet.Item1,
                                           packet.Item2);
                                   }
                               }
                           }).Start();
            }

            return this;
        }

        public void Stop()
        {
            this._isRunning = false;
        }

        private void HandlePacket(INetworkerConnection connection,
            NetworkerPacketBase deserialized,
            byte[] bytes)
        {
            Task.Factory.StartNew(() =>
                                  {
                                      if(!this.packetHandlers.ContainsKey(deserialized.UniqueKey))
                                      {
                                          return;
                                      }

                                      var packetHandlerType = this.packetHandlers[deserialized.UniqueKey];

                                      var packetHandler =
                                          this.container.Resolve<IServerPacketHandler>(packetHandlerType);
                                      packetHandler.Handle(connection, deserialized, bytes);
                                  });
        }

        private void RegisterTypesFromModule(INetworkerPacketHandlerModule packetHandlerModule)
        {
            foreach(var packetHandler in packetHandlerModule.RegisterPacketHandlers())
            {
                this.packetHandlers.Add(packetHandler.Key.Name, packetHandler.Value);
                this.container.RegisterType(packetHandler.Value);
            }
        }
    }
}
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
        private readonly ServerConfiguration _configuration;
        private readonly List<TcpConnection> _connections;
        private readonly DryIocContainer _container;
        public readonly INetworkerLogger Logger;
        private readonly PacketDeserializer packetDeserializer;
        private readonly Dictionary<string, Type> packetHandlers;
        private readonly PacketSerializer packetSerializer;
        private bool _isRunning = true;
        private Socket _tcpSocket;
        private UdpClient _udpClient;
        private Socket _udpSocket;

        protected NetworkerServerBase(ServerConfiguration configuration, INetworkerLogger logger)
        {
            this._configuration = configuration;
            this.Logger = logger;
            this._container = new DryIocContainer();
            this._connections = new List<TcpConnection>();
            this.packetSerializer = new PacketSerializer();
            this.packetDeserializer = new PacketDeserializer();
            this._container.RegisterSingleton(logger);

            this.packetHandlers = this._configuration.PacketHandlers;

            foreach(var packetHandlerModule in this._configuration.PacketHandlerModules)
            {
                this.RegisterTypesFromModule(packetHandlerModule);
            }

            foreach(var packetHandler in this._configuration.PacketHandlers)
            {
                this._container.RegisterType(packetHandler.Value);
            }

            this._configuration.PacketHandlers = null;
            this._configuration.PacketHandlerModules = null;
        }

        public EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        public EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }

        public void Broadcast<T>(T packet)
            where T: NetworkerPacketBase
        {
            if(!this._configuration.UseUdp)
            {
                throw new Exception("Cannot broadcast packet when UDP is not enabled.");
            }

            this._udpSocket.SendTo(this.packetSerializer.Serialize(packet),
                new IPEndPoint(IPAddress.Parse(this._configuration.IpAddresses[0]),
                    this._configuration.UdpPortRemote));
        }

        public INetworkerServer RegisterPacketHandler<TPacketType, TPacketHandlerType>()
            where TPacketHandlerType: IServerPacketHandler
        {
            var handler = typeof(TPacketHandlerType);
            this.packetHandlers.Add(typeof(TPacketType).Name, handler);
            this._container.RegisterType(handler);
            return this;
        }

        public INetworkerServer RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            this.RegisterTypesFromModule(typeof(TPacketHandlerModule));
            return this;
        }

        public void Send<T>(INetworkerConnection connection,
            T packet,
            NetworkerProtocol protocol = NetworkerProtocol.Tcp)
            where T: NetworkerPacketBase
        {
            throw new NotImplementedException();
        }

        public INetworkerServer Start()
        {
            if(this._configuration.UseTcp)
            {
                this.Logger.Trace($"Starting TCP Listener on port {this._configuration.TcpPort}.");

                this._tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._tcpSocket.Bind(new IPEndPoint(IPAddress.Parse(this._configuration.IpAddresses[0]),
                    this._configuration.TcpPort));
                this._tcpSocket.Listen(this._configuration.Advanced.MaxTcpConnections);

                new Thread(() =>
                           {
                               while(this._isRunning)
                               {
                                   if(this._tcpSocket.Poll(10, SelectMode.SelectRead))
                                   {
                                       var acceptedSocket = this._tcpSocket.Accept();

                                       this.Logger.Trace($"New Connection Detected");

                                       this._connections.Add(
                                           new TcpConnection(acceptedSocket));
                                       this.ClientConnected?.Invoke(this,
                                           new TcpConnectionConnectedEventArgs());
                                   }

                                   Thread.Sleep(this._configuration.Advanced.ConnectionPollIntervalMs);
                               }
                           }).Start();

                new Thread(() =>
                           {
                               while(this._isRunning)
                               {
                                   foreach(var connection in this._connections.ToList())
                                   {
                                       if(connection.Socket.Poll(10, SelectMode.SelectRead))
                                       {
                                           var packets =
                                               this.packetDeserializer
                                                   .GetPacketsFromSocket(connection.Socket);

                                           foreach(var packet in packets)
                                           {
                                               Task.Factory.StartNew(() =>
                                                                     {
                                                                         this.HandlePacket(connection,
                                                                             packet.Item1,
                                                                             packet.Item2);
                                                                     });
                                           }
                                       }

                                       Thread.Sleep(this
                                           ._configuration.Advanced.IncomingSocketPollIntervalMs);
                                   }
                               }
                           }).Start();
            }

            if(this._configuration.UseUdp)
            {
                this.Logger.Trace($"Starting UDP Listener on port {this._configuration.UdpPortLocal}.");
                this.Logger.Trace($"Starting UDP Sender on port {this._configuration.UdpPortRemote}.");

                this._udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this._udpClient = new UdpClient(this._configuration.UdpPortLocal);

                new Thread(() =>
                           {
                               while(this._isRunning)
                               {
                                   var result = this._udpClient.ReceiveAsync()
                                                    .Result;

                                   var packets = this.packetDeserializer.GetPacketsFromUdp(result);

                                   foreach(var packet in packets)
                                   {
                                       Task.Factory.StartNew(() =>
                                                             {
                                                                 this.HandlePacket(
                                                                     new UdpConnection(this._udpSocket, result),
                                                                     packet.Item1,
                                                                     packet.Item2);
                                                             });
                                   }

                                   Thread.Sleep(this._configuration.Advanced.IncomingSocketPollIntervalMs);
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
            if(!this.packetHandlers.ContainsKey(deserialized.UniqueKey))
            {
                return;
            }

            var packetHandlerType = this.packetHandlers[deserialized.UniqueKey];

            var packetHandler = this._container.Resolve<IServerPacketHandler>(packetHandlerType);
            packetHandler.Handle(connection, deserialized, bytes);
        }

        private void RegisterTypesFromModule(Type packetHandlerModule)
        {
            var module = (INetworkerPacketBaseHandlerModule)Activator.CreateInstance(packetHandlerModule);

            foreach(var packetHandler in module.RegisterPacketHandlers())
            {
                this.packetHandlers.Add(packetHandler.Key.Name, packetHandler.Value);
            }
        }
    }
}
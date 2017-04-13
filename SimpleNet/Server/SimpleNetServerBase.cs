using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SimpleNet.Common;
using SimpleNet.Interfaces;

namespace SimpleNet.Server
{
    public abstract class SimpleNetServerBase : ISimpleNetServer
    {
        private readonly ServerConfiguration _configuration;
        private readonly List<ISimpleNetConnection> _connections;
        private readonly DryIocContainer _container;
        private readonly ISimpleNetLogger _logger;
        private readonly PacketDeserializer packetDeserializer;
        private readonly Dictionary<string, Type> packetHandlers;
        private readonly PacketSerializer packetSerializer;
        private bool _isRunning = true;
        private Socket _tcpSocket;
        private UdpClient _udpClient;
        private Socket _udpSocket;

        protected SimpleNetServerBase(ServerConfiguration configuration, ISimpleNetLogger logger)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._container = new DryIocContainer();
            this._connections = new List<ISimpleNetConnection>();
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

        public void Broadcast<T>(T packet)
            where T: SimpleNetPacketBase
        {
            if(!this._configuration.UseUdp)
            {
                throw new Exception("Cannot broadcast packet when UDP is not enabled.");
            }

            this._udpSocket.SendTo(this.packetSerializer.Serialize(packet),
                new IPEndPoint(IPAddress.Parse(this._configuration.IpAddresses[0]),
                    this._configuration.UdpPortRemote));
        }

        public ISimpleNetServer RegisterPacketHandler<TPacketType, TPacketHandlerType>()
            where TPacketHandlerType: ISimpleNetServerPacketHandler
        {
            var handler = typeof(TPacketHandlerType);
            this.packetHandlers.Add(typeof(TPacketType).Name, handler);
            this._container.RegisterType(handler);
            return this;
        }

        public ISimpleNetServer RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            this.RegisterTypesFromModule(typeof(TPacketHandlerModule));
            return this;
        }

        public void Send<T>(ISimpleNetConnection connection,
            T packet,
            SimpleNetProtocol protocol = SimpleNetProtocol.Tcp)
            where T: SimpleNetPacketBase
        {
            throw new NotImplementedException();
        }

        public ISimpleNetServer Start()
        {
            if(this._configuration.UseTcp)
            {
                this._logger.Trace($"Starting TCP Listener on port {this._configuration.TcpPort}.");

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

                                       this._logger.Trace($"New Connection Detected");

                                       this._connections.Add(new SimpleNetServerConnection(acceptedSocket));
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

                                       Thread.Sleep(this._configuration.Advanced.IncomingSocketPollIntervalMs);
                                   }
                               }
                           }).Start();
            }

            if(this._configuration.UseUdp)
            {
                this._logger.Trace($"Starting UDP Listener on port {this._configuration.UdpPortLocal}.");
                this._logger.Trace($"Starting UDP Sender on port {this._configuration.UdpPortRemote}.");

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
                                                                 this.HandlePacket(null,
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

        private void HandlePacket(ISimpleNetConnection connection,
            SimpleNetPacketBase deserialized,
            byte[] bytes)
        {
            if(!this.packetHandlers.ContainsKey(deserialized.UniqueKey))
            {
                return;
            }

            var packetHandlerType = this.packetHandlers[deserialized.UniqueKey];

            var packetHandler = this._container.Resolve<ISimpleNetServerPacketHandler>(packetHandlerType);
            packetHandler.Handle(connection, deserialized, bytes);
        }

        private void RegisterTypesFromModule(Type packetHandlerModule)
        {
            var module = (ISimpleNetPacketBaseHandlerModule)Activator.CreateInstance(packetHandlerModule);

            foreach(var packetHandler in module.RegisterPacketHandlers())
            {
                this.packetHandlers.Add(packetHandler.Key.Name, packetHandler.Value);
            }
        }
    }
}
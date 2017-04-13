using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SimpleNet.Common;
using SimpleNet.Interfaces;

namespace SimpleNet.Client
{
    public abstract class SimpleNetClientBase : ISimpleNetClient
    {
        private readonly SimpleNetClientConfiguration _clientConfiguration;
        private readonly ISimpleNetLogger _logger;
        private readonly Dictionary<string, Type> _packetHandlers;
        private readonly IContainerIoc container;
        private readonly bool isRunning;
        private readonly PacketDeserializer packetDeserializer;
        private Socket _tcpSocket;
        private UdpClient _udpClient;

        protected SimpleNetClientBase(SimpleNetClientConfiguration clientConfiguration,
            ISimpleNetLogger logger)
        {
            this._clientConfiguration = clientConfiguration;
            this._logger = logger;
            this.container = new DryIocContainer();
            this.isRunning = true;
            this.packetDeserializer = new PacketDeserializer();
            this.container.RegisterSingleton(logger);

            this._packetHandlers = this._clientConfiguration.PacketHandlers;

            foreach(var packetHandlerModule in clientConfiguration.PacketHandlerModules)
            {
                this.RegisterTypesFromModule(packetHandlerModule);
            }

            foreach(var packetHandler in clientConfiguration.PacketHandlers)
            {
                this.container.RegisterType(packetHandler.Value);
            }

            clientConfiguration.PacketHandlers = null;
            clientConfiguration.PacketHandlerModules = null;
        }

        public ISimpleNetClient Connect()
        {
            if(this._clientConfiguration.UseTcp)
            {
                this._logger.Trace("Connecting to TCP Server");
                this._tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._tcpSocket.Connect(this._clientConfiguration.Ip, this._clientConfiguration.TcpPort);

                new Thread(() =>
                           {
                               while(this.isRunning)
                               {
                                   if(this._tcpSocket.Poll(10, SelectMode.SelectWrite))
                                   {
                                       var packets =
                                           this.packetDeserializer.GetPacketsFromSocket(this._tcpSocket);

                                       foreach(var packet in packets)
                                       {
                                           Task.Factory.StartNew(() =>
                                                                 {
                                                                     this.HandlePacket(packet.Item1,
                                                                         packet.Item2);
                                                                 });
                                       }
                                   }
                                   Thread.Sleep(10);
                               }
                           }).Start();
            }

            if(this._clientConfiguration.UseUdp)
            {
                this._logger.Trace("Listening to UDP broadcasts");
                this._udpClient = new UdpClient(this._clientConfiguration.UdpPortLocal);

                new Thread(() =>
                           {
                               while(this.isRunning)
                               {
                                   var data = this._udpClient.ReceiveAsync()
                                                  .Result;
                                   var packets = this.packetDeserializer.GetPacketsFromUdp(data);

                                   foreach(var packet in packets)
                                   {
                                       Task.Factory.StartNew(() =>
                                                             {
                                                                 this.HandlePacket(packet.Item1,
                                                                     packet.Item2);
                                                             });
                                   }

                                   Thread.Sleep(10);
                               }
                           }).Start();
            }

            return this;
        }

        public ISimpleNetClientPacketReceipt CreatePacket(SimpleNetPacketBase packet)
        {
            throw new NotImplementedException();
        }

        public void Send<T>(T packet, SimpleNetProtocol protocol = SimpleNetProtocol.Tcp)
            where T: SimpleNetPacketBase
        {
            var serializer = new PacketSerializer();
            var serialisedPacket = serializer.Serialize(packet);

            if(protocol == SimpleNetProtocol.Tcp)
            {
                if (!this._clientConfiguration.UseTcp)
                {
                    throw new Exception("Cannot send TCP when TCP not enabled.");
                }

                this._tcpSocket.Send(serialisedPacket);
            }
            else if(protocol == SimpleNetProtocol.Udp)
            {
                if(!this._clientConfiguration.UseUdp)
                {
                    throw new Exception("Cannot send UDP when UDP not enabled.");
                }

                this._udpClient.SendAsync(serialisedPacket,
                    serialisedPacket.Length,
                    this._clientConfiguration.Ip,
                    this._clientConfiguration.UdpPortRemote);
            }
        }

        private void HandlePacket(SimpleNetPacketBase deserialized, byte[] bytes)
        {
            if(!this._packetHandlers.ContainsKey(deserialized.UniqueKey))
            {
                return;
            }

            var packetHandlerType = this._packetHandlers[deserialized.UniqueKey];

            var packetHandler = this.container.Resolve<ISimpleNetClientPacketHandler>(packetHandlerType);

            packetHandler.Handle(deserialized, bytes);
        }

        private void RegisterTypesFromModule(Type packetHandlerModule)
        {
            var module = (ISimpleNetPacketBaseHandlerModule)Activator.CreateInstance(packetHandlerModule);

            foreach(var packetHandler in module.RegisterPacketHandlers())
            {
                this._packetHandlers.Add(packetHandler.Key.Name, packetHandler.Value);
            }
        }
    }
}
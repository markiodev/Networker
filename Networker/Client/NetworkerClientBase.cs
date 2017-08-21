using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.Client
{
    public abstract class NetworkerClientBase : INetworkerClient
    {
        private readonly ClientConfiguration _clientConfiguration;
        private readonly INetworkerLogger _logger;
        private readonly Dictionary<string, Type> _packetHandlers;
        private readonly ClientResponseStore clientResponseStore;
        private readonly bool isRunning;
        private readonly PacketDeserializer packetDeserializer;
        private Socket _tcpSocket;
        private UdpClient _udpClient;

        protected NetworkerClientBase(ClientConfiguration clientConfiguration,
            INetworkerLogger logger,
            IList<INetworkerPacketHandlerModule> packetHandlerModules)
        {
            this._clientConfiguration = clientConfiguration;
            this._logger = logger;
            this.Container = new DryIocContainer();
            this.isRunning = true;
            this.packetDeserializer = new PacketDeserializer();
            this.Container.RegisterSingleton(logger);
            this._packetHandlers = new Dictionary<string, Type>();
            this.clientResponseStore = new ClientResponseStore();

            foreach(var packetHandlerModule in packetHandlerModules)
            {
                this.RegisterTypesFromModule(packetHandlerModule);
            }
        }

        public IContainerIoc Container { get; }

        public INetworkerClient Connect()
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

        public void Send(NetworkerPacketBase packet, NetworkerProtocol protocol = NetworkerProtocol.Tcp)
        {
            var serializer = new PacketSerializer();
            var serialisedPacket = serializer.Serialize(packet);

            if(protocol == NetworkerProtocol.Tcp)
            {
                if(!this._clientConfiguration.UseTcp)
                {
                    throw new Exception("Cannot send TCP when TCP not enabled.");
                }

                this._tcpSocket.Send(serialisedPacket);
            }
            else if(protocol == NetworkerProtocol.Udp)
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

        public IClientPacketReceipt SendAndHandleResponse<TResponseType>(NetworkerPacketBase packet,
            Action<TResponseType> handler)
            where TResponseType: class
        {
            packet.TransactionId = Guid.NewGuid()
                                       .ToString();
            var receipt = new ClientPacketReceipt<TResponseType>(this, packet);
            receipt.HandleResponse(handler);
            this.clientResponseStore.Store(packet.TransactionId, receipt);

            receipt.Send();

            while(this.clientResponseStore.Find(packet.TransactionId) != null)
            {
                Thread.Sleep(5);
            }

            return receipt;
        }

        public IClientPacketReceipt SendAndHandleResponse(NetworkerPacketBase packet)
        {
            return null;
        }

        public IClientPacketReceipt SendAndHandleResponseAsync<TResponseType>(NetworkerPacketBase packet,
            Action<TResponseType> handler)
            where TResponseType: class
        {
            return null;
        }

        private void HandlePacket(NetworkerPacketBase packetBase, byte[] bytes)
        {
            if(!this._packetHandlers.ContainsKey(packetBase.UniqueKey))
            {
                return;
            }

            if(!string.IsNullOrEmpty(packetBase.TransactionId))
            {
                var clientResponse = this.clientResponseStore.Find(packetBase.TransactionId);

                clientResponse?.Invoke(bytes);

                this.clientResponseStore.Remove(packetBase.TransactionId);

                return;
            }

            var packetHandlerType = this._packetHandlers[packetBase.UniqueKey];

            var packetHandler = this.Container.Resolve<IClientPacketHandler>(packetHandlerType);

            packetHandler.Handle(packetBase, bytes);
        }

        private void RegisterTypesFromModule(INetworkerPacketHandlerModule packetHandlerModule)
        {
            foreach(var packetHandler in packetHandlerModule.RegisterPacketHandlers())
            {
                this._packetHandlers.Add(packetHandler.Key.Name, packetHandler.Value);
                this.Container.RegisterType(packetHandler.Value);
            }
        }
    }
}
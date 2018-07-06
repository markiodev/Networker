using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Networker.Common;
using Networker.Interfaces;
using Networker.Server;

namespace Networker.Client
{
    public abstract class NetworkerClientBase : INetworkerClient
    {
        private readonly ClientConfiguration clientConfiguration;
        private readonly ClientResponseStore clientResponseStore;
        private bool isRunning;
        private readonly INetworkerLogger logger;
        private readonly PacketDeserializer packetDeserializer;
        private readonly Dictionary<string, Type> packetHandlers;
        private Socket _tcpSocket;
        private UdpClient _udpClient;

        public EventHandler<Socket> Connected { get; set; }
        public EventHandler<Socket> Disconnected { get; set; }

        protected NetworkerClientBase(ClientConfiguration clientConfiguration,
            INetworkerLogger logger,
            IList<INetworkerPacketHandlerModule> packetHandlerModules)
        {
            this.clientConfiguration = clientConfiguration;
            this.logger = logger;
            this.Container = new ServiceCollectionContainer(new ServiceCollection());
            this.packetDeserializer = new PacketDeserializer();
            this.Container.RegisterSingleton(logger);
            this.packetHandlers = new Dictionary<string, Type>();
            this.clientResponseStore = new ClientResponseStore();

            foreach(var packetHandlerModule in packetHandlerModules)
            {
                this.RegisterTypesFromModule(packetHandlerModule);
            }
        }

        public IContainerIoc Container { get; }

        public INetworkerClient Connect()
        {
            if (this.clientConfiguration.UseTcp)
            {
                this.logger.Trace("Connecting to TCP Server");

                lock (this._tcpLocker)
                {
                    this.isRunning = true;
                }

                new Thread(() =>
                {
                    lock (this._tcpLocker)
                    {
                        this._tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        this._tcpSocket.Connect(this.clientConfiguration.Ip, this.clientConfiguration.TcpPort);
                        this.Connected?.Invoke(this, this._tcpSocket);
                        while (this.isRunning)
                               {
                                       if(!this._tcpSocket.Connected)
                                       {
                                           break;
                                       }

                                       if(this._tcpSocket.Poll(10, SelectMode.SelectWrite))
                                       {
                                           var packets =
                                               this.packetDeserializer.GetPacketsFromSocket(this._tcpSocket);

                                           foreach(var packet in packets)
                                           {
                                               Task.Factory.StartNew(() =>
                                                                     {
                                                                         try
                                                                         {
                                                                             this.HandlePacket(packet.Item1,
                                                                                 packet.Item2);
                                                                         }
                                                                         catch(Exception e)
                                                                         {
                                                                             this.logger.Error(e);
                                                                         }
                                                                     });
                                           }
                                       }
                                   }

                        if(this._tcpSocket.Connected)
                        {
                            this._tcpSocket.Disconnect(false);
                            this._tcpSocket.Close();
                        }

                        this.Disconnected?.Invoke(this, this._tcpSocket);
                    }
                           }).Start();
            }
            
            if(this.clientConfiguration.UseUdp)
            {
                
                this.logger.Trace("Listening to UDP broadcasts");

                new Thread(() =>
                           {
                               lock(this._udpLocker)
                               {
                                   this._udpClient = new UdpClient(this.clientConfiguration.UdpPortLocal);
                                   while(this.isRunning)
                                   {
                                       var data = this._udpClient.ReceiveAsync()
                                                      .Result;
                                       var packets = this.packetDeserializer.GetPacketsFromUdp(data);

                                       foreach(var packet in packets)
                                       {
                                           Task.Factory.StartNew(() =>
                                                                 {
                                                                     try
                                                                     {
                                                                         this.HandlePacket(packet.Item1,
                                                                             packet.Item2);
                                                                     }
                                                                     catch(Exception e)
                                                                     {
                                                                         this.logger.Error(e);
                                                                     }
                                                                 });
                                       }
                                   }

                                   this._udpClient.Close();
                                   this._udpClient = null;
                               }
                           }).Start();
                           
            }
            
            return this;
        }

        private readonly object _udpLocker = new object();
        private readonly object _tcpLocker = new object();

        public void Send<T>(T packet, NetworkerProtocol protocol = NetworkerProtocol.Tcp)
            where T: NetworkerPacketBase
        {
            if(!this.isRunning)
                return;

            var serializer = new PacketSerializer();
            var serialisedPacket = serializer.Serialize(packet);

            if(protocol == NetworkerProtocol.Tcp)
            {
                if(!this.clientConfiguration.UseTcp)
                {
                    throw new Exception("Cannot send TCP when TCP not enabled.");
                }

                this._tcpSocket.Send(serialisedPacket);
            }
            else if(protocol == NetworkerProtocol.Udp)
            {
                if(!this.clientConfiguration.UseUdp)
                {
                    throw new Exception("Cannot send UDP when UDP not enabled.");
                }

                this._udpClient.SendAsync(serialisedPacket,
                    serialisedPacket.Length,
                    this.clientConfiguration.Ip,
                    this.clientConfiguration.UdpPortRemote);
            }
        }

        public IClientPacketReceipt SendAndHandleResponse<T, TResponseType>(T packet,
            Action<TResponseType> handler)
            where TResponseType: class where T: NetworkerPacketBase
        {
            if (!this.isRunning)
                return null;

            packet.TransactionId = Guid.NewGuid()
                                       .ToString();
            var receipt = new ClientPacketReceipt<TResponseType>(this, packet);
            receipt.HandleResponse(handler);
            this.clientResponseStore.Store(packet.TransactionId, receipt);

            receipt.Send();

            //todo: Implement timeout

            while(this.clientResponseStore.Find(packet.TransactionId) != null && this.isRunning)
            {
                Thread.Sleep(1);
            }

            return receipt;
        }

        public long Ping()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            this.SendAndHandleResponse<PingRequestPacket, PingResponsePacket>(new PingRequestPacket(),
                (response) => { sw.Stop(); });

            return sw.ElapsedMilliseconds;
        }

        public void Disconnect()
        {
            if(!this.isRunning)
            {
                return;
            }
            this.isRunning = false;
            this._tcpSocket.Disconnect(false);
            this.clientResponseStore.Clear();
        }

        private void HandlePacket(NetworkerPacketBase packetBase, byte[] bytes)
        {
            if (!string.IsNullOrEmpty(packetBase.TransactionId))
            {
                var clientResponse = this.clientResponseStore.Find(packetBase.TransactionId);

                clientResponse?.Invoke(bytes);

                this.clientResponseStore.Remove(packetBase.TransactionId);

                return;
            }

            if (!this.packetHandlers.ContainsKey(packetBase.UniqueKey))
            {
                return;
            }

            var packetHandlerType = this.packetHandlers[packetBase.UniqueKey];

            var packetHandler = this.Container.Resolve<IClientPacketHandler>(packetHandlerType);

            packetHandler.Handle(packetBase, bytes);
        }

        private void RegisterTypesFromModule(INetworkerPacketHandlerModule packetHandlerModule)
        {
            foreach(var packetHandler in packetHandlerModule.RegisterPacketHandlers())
            {
                this.packetHandlers.Add(packetHandler.Key.Name, packetHandler.Value);
                this.Container.RegisterType(packetHandler.Value);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Demo.Common;
using Networker;
using Networker.Events;

namespace Demo.EventBroker
{
    class Program
    {
        public class EventToProcess
        {
            public int EventTypeId { get; set; }
            public string EventContent { get; set; }
        }

        private static Dictionary<int, bool> _subscribers;
        private static Queue<EventToProcess> _queue;

        static void Main(string[] args)
        {
            _subscribers = new Dictionary<int, bool>();
            _queue = new Queue<EventToProcess>();
            
            IModuleBuilder moduleBuilder = new ModuleBuilder();
            moduleBuilder.RegisterHandler((int)PacketIdentifiers.SubscriberReady, OnSubscriberReadyPacket);

            IServerBuilder serverBuilder = new ServerBuilder();

            IServer server = serverBuilder
                .UseTcp(1000)
                .SetMaxConnections(100)
                .UseModule(moduleBuilder)
                .OnError(Event_LogErrorToConsole)
                .OnClientConnected(Event_ClientConnected)
                .OnClientDisconnected(Event_ClientDisconnected)
                .Build();

            server.Start();

            StartEventGeneratorThread();
            StartEventSubscriberThread(server);
        }

        private static void OnSubscriberReadyPacket(IPacketContext packetContext)
        {
            _subscribers[packetContext.ConnectionId] = true;
        }

        private static void Event_ClientDisconnected(ClientDisconnectedEvent disconnectEvent)
        {
        }

        private static void Event_ClientConnected(ClientConnectedEvent connectEvent)
        {
        }

        private static void Event_LogErrorToConsole(Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }

        private static void StartEventSubscriberThread(IServer server)
        {
            new Thread(() =>
            {
                while (true)
                {
                    foreach (var eventSubscriber in _subscribers.Where(e => e.Value))
                    {
                        if (_queue.TryDequeue(out var eventToProcess))
                        {
                            server.GetConnection(eventSubscriber.Key)
                                .Send(eventToProcess.EventTypeId, eventToProcess.EventContent);
                        }
                    }

                    Console.WriteLine($"There are {server.ActiveConnections} connected clients");
                    Thread.Sleep(1000);
                }
            }).Start();
        }
        private static void StartEventGeneratorThread()
        {
            new Thread(() =>
            {
                while (true)
                {
                    _queue.Enqueue(new EventToProcess
                    {
                        EventTypeId = (int)PacketIdentifiers.ChatMessageToProcess,
                        EventContent = Guid.NewGuid().ToString()
                    });

                    Thread.Sleep(10000);
                }
            }).Start();
        }
    }
}

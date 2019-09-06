using System;
using System.Threading;
using Demo.ChatCommon;
using Networker;
using Networker.Events;
using Networker.Extensions.Json;

namespace Demo.ChatServer
{
    class Program
    {
        private static IServer Server { get; set; }

        static void Main(string[] args)
        {
            IModuleBuilder moduleBuilder = new ModuleBuilder();
            moduleBuilder.RegisterHandler((int)PacketIdentifiers.ChatMessage, OnChatMessage);

            IServerBuilder serverBuilder = new ServerBuilder();

            Server = serverBuilder
                .UseTcp(1000)
                .SetMaxConnections(100)
                .UseModule(moduleBuilder)
                .OnLog(Log_WriteToConsole)
                .OnClientConnected(Event_ClientConnected)
                .OnClientDisconnected(Event_ClientDisconnected)
                .OnError(Log_OnError)
                .Build();

            Server.Start();

            while (Server.State == ServerState.Running)
            {
                Thread.Sleep(1000);
            }
        }

        private static void Log_WriteToConsole(string arg1, string arg2)
        {
            Console.WriteLine($"[{arg1}] {arg2}");
        }

        private static void Event_ClientDisconnected(ClientDisconnectedEvent obj)
        {
            Console.Write("Client Disconnected");
        }

        private static void Log_OnError(Exception error)
        {
            Console.WriteLine(error);
        }

        private static void OnChatMessage(IPacketContext context)
        {
            //Doesn't try to deserialise
            //Relay the chat message bytes to all connected clients
            Server.Broadcast(context.PacketBytes);
            Server.WriteLog("Debug", "Got a chat message");
        }

        private static void Event_ClientConnected(ClientConnectedEvent e)
        {
            Console.WriteLine("Someone connected");
            e.Connection.SendAsJson(new MessageOfTheDay
            {
                Message = "This is the message of the day",
                SetBy = "Admin"
            });
        }
    }
}

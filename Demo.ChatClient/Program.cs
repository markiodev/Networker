using System;
using Demo.ChatCommon;
using Microsoft.Extensions.DependencyInjection;
using Networker;
using Networker.Extensions.Json;

namespace Demo.ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {            
            //Register the packet handlers
            IModuleBuilder moduleBuilder = new ModuleBuilder();
            moduleBuilder.RegisterHandler<MessageOfTheDayHandler>((int)PacketIdentifiers.MessageOfTheDay);
            moduleBuilder.RegisterHandler<ChatMessageHandler>((int)PacketIdentifiers.ChatMessage);

            IClientBuilder clientBuilder = new ClientBuilder();

            clientBuilder
                .UseIp("127.0.0.1")
                .UseTcp(1000)
                .UseModule(moduleBuilder);
            
            var client = clientBuilder.Build();
            var result = client.Connect();

            if (!result.Success)
            {
                Console.WriteLine(result.Reason);
            }
            else
            {
                Console.WriteLine("Connected to the chat room");
                Console.WriteLine("What is your name?");

                string name = Console.ReadLine();

                while (client.ConnectionState == ConnectionState.Connected)
                {
                    var message = Console.ReadLine();

                    client.SendAsJson(new ChatMessage
                    {
                        Name = name,
                        Message = message
                    });
                }
            }
        }
    }
}

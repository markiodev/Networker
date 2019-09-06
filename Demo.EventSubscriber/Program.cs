using System;
using System.Runtime.InteropServices;
using System.Threading;
using Demo.Common;
using Networker;
using Networker.Extensions.Json;

namespace Demo.EventSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            IClientBuilder clientBuilder = new ClientBuilder();

            clientBuilder
                .UseIp("127.0.0.1")
                .UseTcp(1000);
            
            var client = clientBuilder.Build();
            var result = client.Connect();

            client.SendAsJson((int)PacketIdentifiers.ChatMessageToProcess, new ChatMessage
            {
                Message = "Hello"
            });

            while (client.ConnectionState == ConnectionState.Connected)
            {
                client.SendEmpty((int)PacketIdentifiers.SubscriberReady);

                Thread.Sleep(100);
            }
        }
    }
}

using System;
using System.IO.Compression;
using SimpleNet.DryIoc;
using SimpleNet;

namespace SimpleNet.Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ISimpleNetServer server = new SimpleNetServerBuilder()
                .UseIpAddresses(new[] {"127.0.0.1", "localhost"})
                .UsePort(1000)
                .UseTcp()
                .UseDryIoc()
                .RegisterPacketHandler<ChatMessageDispatchPacket, ChatMessageDispatchPacketHandler>()
                .Build<ExampleServer>()
                .Start();

            ISimpleNetClient client = new SimpleNetClientBuilder()
                .UseIp("localhost")
                .UsePort(1000)
                .UseTcp()
                .UseDryIoc()
                .RegisterPacketHandler<ChatMessageReceivedPacket, ChatMessageReceivedPacketHandler>()
                .Build<ExampleClient>()
                .Connect();

            client.Send(new ChatMessageDispatchPacket("Test App", "Hello from the Test App!"));

            client.Send(new ServerInformationRequestPacket()).HandleResponsePacket<ServerInformationResponsePacket>(packet => Console.WriteLine($"I am sync. {packet.MachineName}"));
            client.Send(new ServerInformationRequestPacket()).HandleResponsePacketAsync<ServerInformationResponsePacket>(packet => Console.WriteLine($"I am async. {packet.MachineName}"));
        }
    }
}
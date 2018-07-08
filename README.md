[![Build status](https://ci.appveyor.com/api/projects/status/k2yi64f298bgjxra?svg=true)](https://ci.appveyor.com/project/MarkioE/networker)
[![NuGet](https://img.shields.io/nuget/v/networker.svg)](https://www.nuget.org/packages/Networker/)

# Networker
A simple to use TCP and UDP networking library for .NET.

## Supported Frameworks
* .NET Standard 2.0

### V3 Features
* TCP
* UDP
* Configurable Object Pooling
* Process thousands of requests per second
* Incredibly fast packet serialization using ZeroFormatter
* Dependency Injection using [Service Collection (Default)](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollection?view=aspnetcore-2.1)

## V2
Version 3 included a large rewrite and various breaking changes. To use V2 please see [V2 Branch](https://github.com/MarkioE/Networker/tree/features/v2.1)

## Installing
Install-Package Networker

## Getting Started

Networker uses a client-server architecture for communication.

Many clients can connect to a single server.

### Creating a TCP Server
```csharp
public class Program
    {
        static void Main(string[] args)
        {
            var server = new NetworkerServerBuilder()
            .UseConsoleLogger()
            .UseTcp(1050)
            .RegisterPacketHandler<ChatMessagePacket, ChatMessagePacketHandler>()
            .Build<DefaultServer>()
            .Start();
        }
    }
```

### Creating a TCP Client
```csharp
public class Program
    {
        static void Main(string[] args)
        {
            var client = new NetworkerClientBuilder()
            .UseConsoleLogger()
            .UseIp("127.0.0.1")
            .UseTcp(1050)
            .RegisterPacketHandler<ChatMessagePacket, ChatMessageReceivedPacketHandler>()
            .Build<DefaultClient>()
            .Connect();
        }
    }
```

### Creating a Packet
```csharp
public class ChatMessagePacket : NetworkerPacketBase
    {
        [Index(0)]
        public virtual string Message { get; set; }

        [Index(1)]
        public virtual string Sender { get; set; }
    }
```

### Creating a Client Packet Handler
```csharp
public class ChatMessageReceivedPacketHandler : PacketHandlerBase<ChatMessagePacket>
    {
        public override void Handle(ChatMessagePacket packet)
        {
            var window = MainWindow.Instance;

            window.Dispatcher.Invoke(() =>
                                     {
                                         window.MessageListBox.Items.Add($"{packet.Sender}: {packet.Message}");
                                     });
        }
    }
```

### Creating a Server Packet Handler
```csharp
public class ChatMessagePacketHandler : ServerPacketHandlerBase<ChatMessagePacket>
    {
        private readonly ITcpConnectionsProvider connectionsProvider;

        public ChatMessagePacketHandler(ITcpConnectionsProvider connectionsProvider)
        {
            this.connectionsProvider = connectionsProvider;
        }

        public override void Handle(INetworkerConnection sender, ChatMessagePacket packet)
        {
            foreach(var tcpConnection in this.connectionsProvider.Provide())
            {
                tcpConnection.Send(packet);
            }
        }
    }
```

### Sending a packet from Client to Server
```csharp
this.client.Send(new ChatMessagePacket
                             {
                                 Sender = Environment.MachineName,
                                 Message = this.MessageBox.Text
                             });
```

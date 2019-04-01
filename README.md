[![Build status](https://ci.appveyor.com/api/projects/status/k2yi64f298bgjxra?svg=true)](https://ci.appveyor.com/project/MarkioE/networker)
[![NuGet](https://img.shields.io/nuget/v/networker.svg)](https://www.nuget.org/packages/Networker/)

# Networker
A simple to use TCP and UDP networking library for .NET, designed to be flexible, scalable and FAST.

## Contributing
Get involved and join us in Discord (https://discord.gg/NdEqhAe)

## Supported Frameworks
* .NET Standard 2.0

## Features
* TCP
* UDP
* Socket pooling
* Object pooling
* Process thousands of requests per second
* Dependency Injection using [Micrsoft.Extensions.DependencyInjection.ServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollection?view=aspnetcore-2.1)
* Logging using [Microsoft.Extensions.Logging](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging?view=aspnetcore-2.2)
* Works with [Unity Game Engine](https://unity3d.com)

## Installation
**NuGet Package Manager**
```
Install-Package Networker
```

**You must then install one of the following formatters**

#### ZeroFormatter
```
Install-Package Networker.Extensions.ZeroFormatter
```
#### MessagePack
```
Install-Package Networker.Extensions.MessagePack
```
#### Protobuf-net 
```
Install-Package Networker.Extensions.ProtoBufNet
```
#### JSON (Utf8Json)
```
Install-Package Networker.Extensions.Json
```

## Tutorial - Creating a Basic Unity Multiplayer Game with Networker
[Get started with this tutorial written by the library developer Mark Eastwood](https://markeastwood.net/?p=7)

## Example

Creating a server is easy..

````csharp
var server = new ServerBuilder()
                .UseTcp(1000)
                .UseUdp(5000)
                .RegisterPacketHandlerModule<DefaultPacketHandlerModule>()
                .RegisterPacketHandlerModule<ExamplePacketHandlerModule>()
                .UseZeroFormatter()
                .ConfigureLogging(loggingBuilder =>
                                    {
                                        loggingBuilder.AddConsole();
                                        loggingBuilder.SetMinimumLevel(
                                            LogLevel.Debug);
                                    })
                .Build();

server.Start();
````

You can handle a packet easily using dependency injection, logging and built-in deserialisation.

````csharp
public class ChatPacketHandler : PacketHandlerBase<ChatPacket>
{
	private readonly ILogger<ChatPacketHandler> _logger;

	public ChatPacketHandler(ILogger<ChatPacketHandler> logger)
	{
		_logger = logger;
	}

	public override async Task Process(ChatPacket packet, IPacketContext packetContext)
	{
		_logger.LogDebug("I received the chat message: " + packet.Message);

		packetContext.Sender.Send(new ChatPacket
		{
			Message = "Hey, I got your message!"
		});
	}
}
````

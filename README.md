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
* Socket Pooling
* Object Pooling
* Process thousands of requests per second
* Dependency Injection using [Micrsoft.Extensions.DependencyInjection.ServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollection?view=aspnetcore-2.1)
* Logging using [Microsoft.Extensions.Logging](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging?view=aspnetcore-2.2)
* Works with [Unity Game Engine](https://unity3d.com)

## Installation
V3 is currently in pre-release. Get the latest build below.
**NuGet Package Manager**
```
Install-Package Networker -IncludePrerelease
```

**You must then install one of the following formatters**

#### ZeroFormatter 
[Networker.Formatter.ZeroFormatter](https://www.nuget.org/packages/Networker.Formatter.ZeroFormatter)
```
Install-Package Networker.Formatter.ZeroFormatter
```
#### Protobuf-net 
[Networker.Formatter.ProtoBufNet](https://www.nuget.org/packages/Networker.Formatter.ProtoBufNet)
```
Install-Package Networker.Formatter.ProtoBufNet
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
public class PingPacketHandler : PacketHandlerBase<PingPacket>
{
    private readonly ILogger logger;

    public PingPacketHandler(ILogger<PingPacketHandler> logger)
    {
        this.logger = logger;
    }

    public override async Task Process(PingPacket packet, ISender sender)
    {
        this.logger.Debug("Received a ping packet from " + sender.EndPoint);
    }
}
````

## Older Versions
Version 3 included a large rewrite and various breaking changes. To use V2 please see [V2 Branch](https://github.com/MarkioE/Networker/tree/features/v2.1)

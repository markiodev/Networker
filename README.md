Note: Networker is currently a WIP. Not suitable for production.

# Networker
A simple to use TCP and UDP networking library for .NET Core and .NET Framework.

## Features
* TCP
* UDP
* Low memory footprint
* Handle thousands of simultaneous connections
* Incredibly fast serialization using ZeroFormatter
* Plug in your choice of IOC
* Plug in your choice of logging
* Encryption (WIP)

## Supported Frameworks
* .NET Standard 1.6
* .NET Core
* .NET Framework 4.5.2

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
            var server = new NetworkerServerBuilder().UseConsoleLogger()
                                                     .UseTcp(1050)
                                                     .Build<DefaultServer>();
            server.Start();
        }
    }
```

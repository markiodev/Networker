![CI Status](https://travis-ci.org/MarkioE/SimpleNet.svg?branch=master)

# SimpleNet
A simple to use TCP and UDP networking library for .NET Core and .NET Framework.

## Features
* TCP
* UDP
* Incredibly fast serialization using ZeroNetFormatter
* Encryption (WIP)

## Supported Frameworks
* .NET Standard 1.6
* .NET Core
* .NET Framework 4.5.2

## Installing
Install-Package SimpleNet

## Getting Started

### Create a TCP Server

`new SimpleNetServerBuilder().UseConsoleLogger()
                                                     .UseIpAddresses(new[] {"127.0.0.1"})
                                                     .UseTcp(1000)
                                                     .RegisterPacketHandler<ChatMessageDispatchPacket, ChatMessageDispatchPacketHandler>()
                                                     .Build<DefaultServer>()
                                                     .Start();`

#### Create a Packet

#### Create a Packet Handler

### Create a Client
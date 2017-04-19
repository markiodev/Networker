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

#### Latency Simulator
Install-Package Networker.LatencySimulator

## Getting Started

Networker uses a client-server architecture for communication.

Many clients can connect to a single server.

[![Build status](https://ci.appveyor.com/api/projects/status/k2yi64f298bgjxra?svg=true)](https://ci.appveyor.com/project/MarkioE/networker)
[![NuGet](https://img.shields.io/nuget/v/networker.svg)](https://www.nuget.org/packages/Networker/)

# Networker
A simple to use TCP and UDP networking library for .NET, designed to be flexible, scalable and FAST.

## Supported Frameworks
* .NET Standard 2.0

## Features
* TCP
* UDP
* Socket Pooling
* Object Pooling
* Process thousands of requests per second
* Incredibly fast packet serialization using ZeroFormatter
* Dependency Injection using [Service Collection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollection?view=aspnetcore-2.1)
* Works with [Unity Game Engine](https://unity3d.com)

## Planned Features
* Packet Encryption

## Installation
**NuGet Package Manager**
```
Install-Package Networker
```

**dotnet cli**
```
dotnet add package Networker
```

## Getting Started
Find more information about how to get started on our [Wiki](https://github.com/MarkioE/Networker/wiki) or view the [Examples](https://github.com/MarkioE/Networker/tree/master/Examples).

## Older Versions
Version 3 included a large rewrite and various breaking changes. To use V2 please see [V2 Branch](https://github.com/MarkioE/Networker/tree/features/v2.1)

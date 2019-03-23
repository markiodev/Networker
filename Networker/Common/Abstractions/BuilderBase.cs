using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Networker.Common.Abstractions
{
	public abstract class BuilderBase<TBuilder, TResult, TBuilderOptions> : IBuilder<TBuilder, TResult>
		where TBuilder : class, IBuilder<TBuilder, TResult> where TBuilderOptions : class, IBuilderOptions
	{
		private IConfiguration configuration;

		private Action<ILoggingBuilder> loggingBuilder;

		//Modules
		protected PacketHandlerModule module;
		protected List<IPacketHandlerModule> modules;

		//Builder Options
		protected TBuilderOptions options;

		//Service Collection
		protected IServiceCollection serviceCollection;
		private Action<IServiceCollection> serviceCollectionFactory;
		protected Func<IServiceProvider> serviceProviderFactory;

		public BuilderBase()
		{
			options = Activator.CreateInstance<TBuilderOptions>();
			serviceCollection = new ServiceCollection();
			modules = new List<IPacketHandlerModule>();
			module = new PacketHandlerModule();
			modules.Add(module);
		}

		public abstract TResult Build();

		public TBuilder ConfigureLogging(Action<ILoggingBuilder> loggingBuilder)
		{
			this.loggingBuilder = loggingBuilder;
			return this as TBuilder;
		}

		public IServiceCollection GetServiceCollection()
		{
			return serviceCollection;
		}

		public IServiceProvider GetServiceProvider()
		{
			var serviceProvider = serviceProviderFactory != null
				? serviceProviderFactory.Invoke()
				: serviceCollection.BuildServiceProvider();
			try
			{
				PacketSerialiserProvider.PacketSerialiser = serviceProvider.GetService<IPacketSerialiser>();
			}
			catch (Exception ex)
			{
				throw new Exception("No packet serialiser has been configured for Networker");
			}

			if (PacketSerialiserProvider.PacketSerialiser == null)
				throw new Exception("No packet serialiser has been configured for Networker");

			var packetHandlers = serviceProvider.GetService<IPacketHandlers>();
			foreach (var packetHandlerModule in modules)
			foreach (var packetHandler in packetHandlerModule.GetPacketHandlers())
				packetHandlers.Add(
					PacketSerialiserProvider.PacketSerialiser.CanReadName ? packetHandler.Key.Name : "Default",
					(IPacketHandler) serviceProvider.GetService(packetHandler.Value));

			if (!PacketSerialiserProvider.PacketSerialiser.CanReadName && packetHandlers.GetPacketHandlers()
				    .Count > 1)
				throw new Exception(
					"A PacketSerialiser which cannot identify a packet can only support up to one packet type");

			return serviceProvider;
		}

		//public TBuilder RegisterMiddleware<T>()
		//	where T : class, IMiddlewareHandler
		//{
		//	serviceCollection.AddSingleton<IMiddlewareHandler, T>();
		//	return this as TBuilder;
		//}

		public TBuilder RegisterPacketHandler<TPacket, TPacketHandler>()
			where TPacket : class where TPacketHandler : IPacketHandler
		{
			module.AddPacketHandler<TPacket, TPacketHandler>();
			return this as TBuilder;
		}

		public TBuilder RegisterPacketHandlerModule(IPacketHandlerModule packetHandlerModule)
		{
			modules.Add(packetHandlerModule);
			return this as TBuilder;
		}

		public TBuilder RegisterPacketHandlerModule<T>()
			where T : IPacketHandlerModule
		{
			modules.Add(Activator.CreateInstance<T>());
			return this as TBuilder;
		}

		public TBuilder RegisterTypes(Action<IServiceCollection> serviceCollection)
		{
			serviceCollectionFactory = serviceCollection;
			return this as TBuilder;
		}

		public TBuilder SetPacketBufferSize(int size)
		{
			options.PacketSizeBuffer = size;
			return this as TBuilder;
		}

		public TBuilder SetServiceCollection(IServiceCollection serviceCollection,
			Func<IServiceProvider> serviceProviderFactory = null)
		{
			this.serviceCollection = serviceCollection;
			this.serviceProviderFactory = serviceProviderFactory;
			return this as TBuilder;
		}

		public TBuilder UseConfiguration(IConfiguration configuration)
		{
			this.configuration = configuration;
			serviceCollection.AddSingleton(configuration);
			return this as TBuilder;
		}

		public TBuilder UseConfiguration<T>(IConfiguration configuration)
			where T : class
		{
			this.configuration = configuration;
			serviceCollection.AddSingleton(configuration);
			serviceCollection.Configure<T>(configuration);
			return this as TBuilder;
		}

		public TBuilder UseTcp(int port)
		{
			options.TcpPort = port;
			return this as TBuilder;
		}

		public TBuilder UseUdp(int port)
		{
			options.UdpPort = port;
			return this as TBuilder;
		}

		protected void SetupSharedDependencies()
		{
			foreach (var packetHandlerModule in modules)
			foreach (var packetHandler in packetHandlerModule.GetPacketHandlers())
				serviceCollection.AddSingleton(packetHandler.Value);

			serviceCollection.AddSingleton(options);
			serviceCollection.AddSingleton<IPacketHandlers, PacketHandlers>();

			if (loggingBuilder == null) loggingBuilder = loggerBuilderFactory => { };

			serviceCollection.AddLogging(loggingBuilder);
			serviceCollectionFactory?.Invoke(serviceCollection);
		}
	}
}
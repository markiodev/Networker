using System;
using System.Linq;
using System.Reflection;
using SimpleNet.Interfaces;
using SimpleNet.Server;

namespace SimpleNet.Helpers
{
    public static class PacketHandlerHelper
    {
        public static ISimpleNetServerBuilder AutoRegisterPacketHandlers(this ISimpleNetServerBuilder builder,
            Assembly assembly)
        {
            var packetHandlerType = typeof(IServerPacketHandler);

            var types = assembly.GetTypes()
                                .Where(p => packetHandlerType.GetTypeInfo()
                                                             .IsAssignableFrom(p));

            foreach(var type in types)
            {
                var instance = (IServerPacketHandler)Activator.CreateInstance(type);

                builder.RegisterPacketHandler(instance.GetPacketType()
                                                      .Name,
                    type);
            }
            return builder;
        }

        public static ISimpleNetClientBuilder AutoRegisterPacketHandlers(this ISimpleNetClientBuilder builder,
            Assembly assembly)
        {
            var packetHandlerType = typeof(IClientPacketHandler);

            var types = assembly.GetTypes()
                                .Where(p => packetHandlerType.GetTypeInfo()
                                                             .IsAssignableFrom(p));

            foreach(var type in types)
            {
                var instance = (IClientPacketHandler)Activator.CreateInstance(type);

                builder.RegisterPacketHandler(instance.GetPacketType()
                                                      .Name,
                    type);
            }
            return builder;
        }
    }
}
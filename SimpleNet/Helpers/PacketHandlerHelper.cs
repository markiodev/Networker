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
            var packetHandlerType = typeof(ISimpleNetServerPacketHandler);

            var types = assembly.GetTypes()
                                .Where(p => packetHandlerType.GetTypeInfo()
                                                             .IsAssignableFrom(p));

            foreach(var type in types)
            {
                var instance = (ISimpleNetServerPacketHandler)Activator.CreateInstance(type);

                builder.RegisterPacketHandler(instance.GetPacketType()
                                                      .Name,
                    type);
            }
            return builder;
        }

        public static ISimpleNetClientBuilder AutoRegisterPacketHandlers(this ISimpleNetClientBuilder builder,
            Assembly assembly)
        {
            var packetHandlerType = typeof(ISimpleNetClientPacketHandler);

            var types = assembly.GetTypes()
                                .Where(p => packetHandlerType.GetTypeInfo()
                                                             .IsAssignableFrom(p));

            foreach(var type in types)
            {
                var instance = (ISimpleNetClientPacketHandler)Activator.CreateInstance(type);

                builder.RegisterPacketHandler(instance.GetPacketType()
                                                      .Name,
                    type);
            }
            return builder;
        }
    }
}
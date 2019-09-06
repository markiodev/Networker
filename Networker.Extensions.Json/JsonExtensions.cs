using Utf8Json;

namespace Networker.Extensions.Json
{
    public static class JsonExtensions
    {
        public static void SendAsJson<T>(this IClient client, int packetIdentifier, T packet)
        {
            client.Send(packetIdentifier, JsonSerializer.Serialize(packet));
        }

        public static void SendAsJson<T>(this IConnection connection, int packetIdentifier, T packet)
        {
            connection.Send(packetIdentifier, JsonSerializer.Serialize(packet));

        }
        public static void SendAsJson<T>(this IClient client, T packet) where T : PacketBase
        {
            client.Send(packet.PacketTypeId, JsonSerializer.Serialize(packet));
        }

        public static void SendAsJson<T>(this IConnection connection, T packet) where T : PacketBase
        {
            connection.Send(packet.PacketTypeId, JsonSerializer.Serialize(packet));
        }
    }
}
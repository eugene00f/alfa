using Confluent.Kafka;
using System.Text;

namespace Kafka.Client.Serializer;

public class Utf8StringSerializer : ISerializer<string>, IDeserializer<string>
{
    public byte[] Serialize(string data, SerializationContext context)
    {
        return Encoding.UTF8.GetBytes(data);
    }

    public string Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return Encoding.UTF8.GetString(data);
    }
}

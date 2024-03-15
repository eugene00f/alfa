using Confluent.Kafka;
using System.Net;

namespace Kafka.Client.Settings;

public class MessageBrokerSettings : IMessageBrokerSettings
{
   public string? Topics { get; set; }

    public string? Hosts { get; set; }

    public string? ConsumerGroupId { get; set; }

    public string? ConsumerClientId { get; set; }
    
    public AutoOffsetReset? AutoOffsetResetType { get; set; }  = null;

    public ProducerConfig GetProducerConfig() => new ()
    {
        BootstrapServers = string.Join(",", Hosts ?? string.Empty),
        ClientId = Dns.GetHostName(),
        Acks = Acks.All
    };

    public ConsumerConfig GetConsumerConfig() => new()
    {
        BootstrapServers = string.Join(",", Hosts ?? string.Empty),
        GroupId = ConsumerGroupId,
        AutoOffsetReset = AutoOffsetResetType ?? AutoOffsetReset.Latest,
        ClientId = ConsumerClientId,
        EnableAutoCommit = true,

    };
}

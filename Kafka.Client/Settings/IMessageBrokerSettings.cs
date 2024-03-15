using System.Text;
using Confluent.Kafka;


namespace Kafka.Client.Settings;

public interface IMessageBrokerSettings
{
    string? Topics { get; }

    string? Hosts { get; }

    string? ConsumerGroupId { get; set; }

    string? ConsumerClientId { get; set; }
    AutoOffsetReset? AutoOffsetResetType { get; set; }

    ProducerConfig GetProducerConfig();

    ConsumerConfig GetConsumerConfig();

}

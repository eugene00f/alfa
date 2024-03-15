using Confluent.Kafka;
using Kafka.Client.Settings;
using System.Text;

namespace Kafka.Client.Producer;

internal class KafkaProducer<TObject, TSerializer>
    where TSerializer: ISerializer<TObject>, new()
    where TObject : class
{
    private readonly IProducer<Null, TObject> _producer;

    public KafkaProducer(IMessageBrokerSettings settings)
    {
        var config = settings.GetProducerConfig();
        _producer = new ProducerBuilder<Null, TObject>(config)
            .SetValueSerializer(new TSerializer())
            .Build();
    }

    internal async Task<bool> ProduceAsync(string topic,
        TObject value,
        Dictionary<string, string>? headers = null,
        CancellationToken token = default)
    {
        Headers? headersToAttach = null;
        if (headers != null)
        {
            headersToAttach = new Headers();
            foreach (var key in headers.Keys)
                headersToAttach.Add(key, Encoding.UTF8.GetBytes(headers[key]));
        }

        var result = await _producer.ProduceAsync(topic, new Message<Null, TObject>
        {
            Value = value,
            Headers = headersToAttach
        }, token);
        return result is { Status: PersistenceStatus.Persisted };
    }
}

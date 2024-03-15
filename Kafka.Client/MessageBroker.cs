using System.Text;
using Confluent.Kafka;
using Kafka.Client.Consumer;
using Kafka.Client.Producer;
using Kafka.Client.Settings;

namespace Kafka.Client;

public class MessageBroker<TObject, TSerializer> : IMessageBroker<TObject>
    where TObject : class
    where TSerializer : ISerializer<TObject>, IDeserializer<TObject>, new()
{
    private readonly KafkaProducer<TObject, TSerializer> _producer;
    private readonly KafkaConsumer<TObject, TSerializer> _consumer;
    private readonly IMessageBrokerSettings _settings;

    public IMessageBrokerSettings Settings => _settings;

    public IOffsetManagedConsumer<TObject>? Consumer => _consumer;

    public MessageBroker(IMessageBrokerSettings settings)
    {
        _settings = settings;
        _producer = new KafkaProducer<TObject, TSerializer>(_settings);
        _consumer = new KafkaConsumer<TObject, TSerializer>(_settings);
    }

    public async Task<bool> ProduceAsync(string topic,
        TObject[] values,
        Dictionary<string, string>? headers = null,
        CancellationToken token = default)
    {
        foreach (var value in values)
        {
            var result = await _producer.ProduceAsync(topic, value, headers, token);
            if (!result)
                return false;
        }

        return true;
    }

    public async Task<bool> ProduceAsync(string topic,
        TObject values,
        Dictionary<string, string>? headers = null,
        CancellationToken token = default)
    {
        return await ProduceAsync(topic, new[] { values }, headers, token);
    }

    public async Task TryConsumingLastMessageAsync(IEnumerable<string> topics,
        Action<MessageInfo<TObject>> newMessageHandler,
        CancellationToken token = default)
    {
        if (_consumer == null)
            throw new Exception("Consumer has not been created.");

        await _consumer.TryConsumeLastMessageAsync(topics,
            (res) => newMessageHandler.Invoke(
                new MessageInfo<TObject>(res.Message.Value,
                    res.Topic,
                    res.Offset.Value,
                    res.Partition.Value,
                    res.Message.Timestamp.UtcDateTime)
                {
                    Headers = res.Message.Headers.ToDictionary(o => o.Key,
                        o => Encoding.UTF8.GetString(o.GetValueBytes()))
                })
            , token);
    }

    public async Task StartConsumingAsync(IEnumerable<string> topics,
        Action<MessageInfo<TObject>> newMessageHandler,
        CancellationToken token = default)
    {
        if (_consumer == null)
            throw new Exception("Consumer has not been created.");

        await _consumer.ConsumeAsync(topics,
            (res) => newMessageHandler.Invoke(
                new MessageInfo<TObject>(res.Message.Value,
                    res.Topic,
                    res.Offset.Value,
                    res.Partition.Value,
                    res.Message.Timestamp.UtcDateTime)
                {
                    Headers = res.Message.Headers.ToDictionary(o => o.Key,
                        o => Encoding.UTF8.GetString(o.GetValueBytes()))
                })
            , token);
    }

    public async Task StartConsumingAsync(IEnumerable<string> topics,
        Func<MessageInfo<TObject>, Task> newMessageHandler,
        CancellationToken token = default)
    {
        if (_consumer == null)
            throw new Exception("Consumer has not been created.");

        await _consumer.ConsumeAsync(topics,
            (res) => newMessageHandler.Invoke(
                new MessageInfo<TObject>(res.Message.Value,
                    res.Topic,
                    res.Offset.Value,
                    res.Partition.Value,
                    res.Message.Timestamp.UtcDateTime)
                {
                    Headers = res.Message.Headers.ToDictionary(o => o.Key,
                        o => Encoding.UTF8.GetString(o.GetValueBytes()))
                })
            , token);
    }
}

using Confluent.Kafka;
using Kafka.Client.Settings;
using System.Text;

namespace Kafka.Client.Consumer;

internal class KafkaConsumer<TObject, TSerializer> : IOffsetManagedConsumer<TObject>
    where TSerializer : IDeserializer<TObject>, new()
    where TObject : class
{
    private readonly IConsumer<Ignore, TObject> _consumer;

    public KafkaConsumer(IMessageBrokerSettings settings)
    {
        var config = settings.GetConsumerConfig();
        _consumer = new ConsumerBuilder<Ignore, TObject>(config)
            .SetValueDeserializer(new TSerializer())
            .Build();
    }
    
    internal async Task ConsumeAsync(IEnumerable<string> topics,
        Action<ConsumeResult<Ignore, TObject>> handler,
        CancellationToken token)
    {
        _consumer.Subscribe(topics);

        await Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(token);
                handler(consumeResult);
            }
        }, token);

        _consumer.Close();
    }

    internal async Task TryConsumeLastMessageAsync(IEnumerable<string> topics,
        Action<ConsumeResult<Ignore, TObject>> handler,
        CancellationToken token)
    {
        _consumer.Subscribe(topics);

        await Task.Run(() =>
        {
                var consumeResult = _consumer.Consume(token);
                handler(consumeResult);
        }, token);

        _consumer.Close();
    }
    

    public ConsumeResult<Ignore, TObject> Consume(CancellationToken cancellation = default(CancellationToken))
    {
        return _consumer.Consume(cancellation);
    }

    public void Seek(TopicPartitionOffset tpo)
    {
        _consumer.Seek(tpo);
    }

    public WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition)
    {
        return _consumer.GetWatermarkOffsets(topicPartition);
    }

    public WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeSpan)
    {
        return _consumer.QueryWatermarkOffsets(topicPartition, timeSpan);
    }

    public void Assign(TopicPartitionOffset tpo)
    {
        _consumer.Assign(tpo);
    }
    public List<TopicPartitionOffset> Commit()
    {  
        return _consumer.Commit();
    }

    public void Commit(IEnumerable<TopicPartitionOffset> offsets)
    {
        _consumer.Commit(offsets);
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }
}

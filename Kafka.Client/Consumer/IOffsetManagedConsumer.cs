using Confluent.Kafka;

namespace Kafka.Client.Consumer;

public interface IOffsetManagedConsumer<TObject>
{
    void Seek(TopicPartitionOffset tpo);

    void Assign(TopicPartitionOffset tpo);

    ConsumeResult<Ignore, TObject> Consume (CancellationToken cancellationToken = default (CancellationToken));

    List<TopicPartitionOffset> Commit();

    void Commit(IEnumerable<TopicPartitionOffset> offsets);

    WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition);

    WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeSpan);
}

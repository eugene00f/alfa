using Kafka.Client.Consumer;
using Kafka.Client.Settings;

namespace Kafka.Client
{
    public interface IMessageBroker<TObject>
    {
        Task<bool> ProduceAsync(string topic,
            TObject[] value,
            Dictionary<string, string>? headers = null,
            CancellationToken token = default);

        Task<bool> ProduceAsync(string topic,
            TObject value,
            Dictionary<string, string>? headers = null,
            CancellationToken token = default);

        public IMessageBrokerSettings Settings { get; }

        IOffsetManagedConsumer<TObject>? Consumer { get; }

        Task StartConsumingAsync(IEnumerable<string> topics,
            Action<MessageInfo<TObject>> newMessageHandler,
            CancellationToken token = default);

        Task TryConsumingLastMessageAsync(IEnumerable<string> topics,
            Action<MessageInfo<TObject>> newMessageHandler,
            CancellationToken token = default);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka.Client;

public class MessageInfo<T>
{
    public MessageInfo (T value,
        string topic,
        long offset,
        int partition,
        DateTime timestamp)
    {
        Value = value;
        Offset = offset;
        Topic = topic;
        Partition = partition;
        Timestamp = timestamp;
    }
    public T Value { get; set; }

    public string Topic { get; set; }

    public long Offset { get; set; }

    public int Partition { get; set; }

    public Dictionary<string, string>? Headers { get; set; }

    public DateTime Timestamp { get; set; }
}

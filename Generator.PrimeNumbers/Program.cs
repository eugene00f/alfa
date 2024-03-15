using Autofac;
using Kafka.Client;
using Kafka.Client.Serializer;
using Kafka.Client.Settings;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Newtonsoft.Json;
using Confluent.Kafka;
using PrimeNumbersGenerator.Contracts;


public class Program
{
    private static int _lastNumber;
    public static async Task Main(string[] args)
    {
        var configuration = CreateConfigurationBuilder(args);
        var container = BuildContainer(configuration);
        var messageBroker = container.Resolve<IMessageBroker<string>>();
        await GenerateNextPrimesAndPublishAsync(messageBroker, messageBroker.Settings.Topics);

        Console.ReadKey();
    }

    
    public static IContainer BuildContainer(IConfiguration configuration)
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<MessageBroker<string, Utf8StringSerializer>>()
            .As<IMessageBroker<string>>();

        builder.Register(ctx =>
            {
                var massegeBrokerSettings = new MessageBrokerSettings()
                {
                    Topics = configuration.GetValue<string>("Kafka:Topics"),
                    Hosts = configuration.GetValue<string>("Kafka:Hosts"),
                    ConsumerGroupId = configuration.GetValue<string>("Kafka:ConsumerGroupId")
            
                };
                return massegeBrokerSettings;
            })
            .As<IMessageBrokerSettings>().SingleInstance();
        return builder.Build();
    }

    private static IConfiguration CreateConfigurationBuilder(string[] args)
    {
        return new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: false)
            .Build();
    }

    public static async Task GenerateNextPrimesAndPublishAsync(IMessageBroker<string> messageBroker, string topic, int numbersInBatch = 20)
    {
        var previousNubmer = 2;
        WatermarkOffsets watermarkOffsets = messageBroker.Consumer.QueryWatermarkOffsets(new TopicPartition(topic, new Partition(0)), TimeSpan.FromSeconds(3));
        Console.WriteLine($"watermarkOffsets high value {watermarkOffsets.High.Value} ");
        if (watermarkOffsets.High.Value > 0)
        {
            var tpo = new TopicPartitionOffset(topic, new Partition(0), new Offset(watermarkOffsets.High.Value - 1));
            messageBroker.Consumer.Assign(tpo);
            Console.WriteLine($"Try to consume");
            await messageBroker.TryConsumingLastMessageAsync(new List<string> { topic }, Handler);
            previousNubmer = _lastNumber;
        }
        
        while (true)
        {
            int currentNumbersInBatch = 0;
            for (int i = previousNubmer + 1; currentNumbersInBatch < numbersInBatch; i++)
            {
                var generatedTime = DateTime.UtcNow;
                if (IsPrime(i))
                {
                    ++currentNumbersInBatch;

                    var message = JsonConvert.SerializeObject(new Message
                    {
                        Number = i,
                        GeneratedTime = generatedTime
                    });

                    Console.WriteLine($"Try to publish {message} ");
                    await messageBroker.ProduceAsync(topic, message);
                    Console.WriteLine($"{message} published");

                    if (currentNumbersInBatch != numbersInBatch)
                    {
                        await Task.Delay(i % 17);
                    }
                    else
                    {
                        previousNubmer = i;
                        await Task.Delay(1000 - DateTime.UtcNow.Microsecond);
                    }
                }
            }
        }
    }

    public static bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number == 2) return true;
        for (int i = 2; i * i <= number; i++)
            if (number % i == 0) return false;
        return true;
    }

    private static void Handler(MessageInfo<string> messageInfo)
    {
        var message = JsonConvert.DeserializeObject<Message>(messageInfo.Value);
        _lastNumber = message?.Number ?? 2;
    }
}
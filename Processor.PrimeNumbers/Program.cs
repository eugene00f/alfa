using Autofac;
using Kafka.Client;
using Kafka.Client.Serializer;
using Kafka.Client.Settings;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Newtonsoft.Json;
using ProcessorPrimeNumbers.Adapters;
using ProcessorPrimeNumbers.Adapters.Clickhouse;
using ProcessorPrimeNumbers.Contracts;
using MediatR;
using MediatR.Extensions.Autofac.DependencyInjection;
using ProcessorPrimeNumbers.Adapters.Commands;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;


public class Program
{
    private static IMediator _mediator;
    public static async Task Main(string[] args)
    {
        var configuration = CreateConfigurationBuilder(args);
        var container = BuildContainer(configuration);
        _mediator = container.Resolve<IMediator>();
        var messageBroker = container.Resolve<IMessageBroker<string>>();
        var ClickhouseConnectionProvider = container.Resolve<IClickhouseConnectionProvider>();

        await messageBroker.StartConsumingAsync(new List<string> { messageBroker.Settings.Topics }, HandlerAsync);

        Console.ReadKey();
    }

    public static IContainer BuildContainer(IConfiguration configuration)
    {
        var builder = new ContainerBuilder();

        var connectionString = configuration.GetConnectionString("ClickhouseDb");
        if(string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("String is null or empty", nameof(connectionString));
        }

        builder.RegisterType<ClickhouseConnectionProvider>()
            .WithParameter("connectionString", connectionString)
            .As(typeof(IClickhouseConnectionProvider))
            .SingleInstance();

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

        var mediatrConfiguration = MediatRConfigurationBuilder
            .Create(typeof(SavePrimeNumber).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithRegistrationScope(RegistrationScope.Scoped)
            .Build();

        builder.RegisterMediatR(mediatrConfiguration);

        return builder.Build();
    }

    private static IConfiguration CreateConfigurationBuilder(string[] args)
    {
        return new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: false)
            .Build();
    }

    private static async void HandlerAsync(MessageInfo<string> messageInfo)
    {
        var message = JsonConvert.DeserializeObject<Message>(messageInfo.Value);
        await _mediator.Send(new SavePrimeNumber(message), CancellationToken.None);
    }
}
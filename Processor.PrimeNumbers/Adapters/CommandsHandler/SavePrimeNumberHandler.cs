using MediatR;
using Dapper;
using ProcessorPrimeNumbers.Adapters.Commands;
using ProcessorPrimeNumbers.Adapters.Pocos;
using System.Diagnostics.CodeAnalysis;
using ProcessorPrimeNumbers.Adapters.Clickhouse;


namespace ProcessorPrimeNumbers.Adapters.CommandsHandler;

internal class SavePrimeNumberHandler : DbHandler, IRequestHandler<SavePrimeNumber>
{
    private const string SavePrimeNumberCommand = @"INSERT INTO alfn.primes (
        generated_time, published_time, nickname, number)
        VALUES (@GeneratedTime, @PublishedTime, @Nickname, @Number)";

    public SavePrimeNumberHandler([NotNull] IClickhouseConnectionProvider connectionProvider) : base (connectionProvider)
    {

    }

    public async Task Handle(SavePrimeNumber request, CancellationToken cancellationToken)
    {
        await using var connection = await GetConnectionAsync(cancellationToken);
        var primeNumberPoco = new PrimeNumberPoco
        {
            Number = request.Message.Number,
            Nickname = request.Message.Nickname,
            GeneratedTime = request.Message.GeneratedTime,
            PublishedTime = request.Message.PublishedTime,
        };

        await connection.ExecuteScalarAsync(SavePrimeNumberCommand, primeNumberPoco);
    }
}

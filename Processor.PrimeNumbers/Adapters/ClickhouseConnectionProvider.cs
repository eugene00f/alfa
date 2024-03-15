using ClickHouse.Client.ADO;
using ProcessorPrimeNumbers.Adapters.Clickhouse;
using Dapper;

namespace ProcessorPrimeNumbers.Adapters;

internal class ClickhouseConnectionProvider: IClickhouseConnectionProvider
{
    private readonly string _connectionString;

    public ClickhouseConnectionProvider(string connectionString)
    {
        _connectionString = connectionString;
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public ClickHouseConnection Create() => new (_connectionString);
}

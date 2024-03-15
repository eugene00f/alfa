using ClickHouse.Client.ADO;

namespace ProcessorPrimeNumbers.Adapters.Clickhouse;

internal interface IClickhouseConnectionProvider
{
    ClickHouseConnection Create();
}

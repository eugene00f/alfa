using ClickHouse.Client.ADO;
using ProcessorPrimeNumbers.Adapters.Clickhouse;
using System.Diagnostics.CodeAnalysis;

namespace ProcessorPrimeNumbers.Adapters;

internal abstract class DbHandler
{
    private readonly IClickhouseConnectionProvider _connectionProvider;
    protected DbHandler([NotNull] IClickhouseConnectionProvider connectionProvider)
    {
        this._connectionProvider = connectionProvider;
    }
    protected async Task<ClickHouseConnection> GetConnectionAsync(CancellationToken token)
    {
        var conn = _connectionProvider.Create();
        await conn.OpenAsync(token);
        return conn;
    }
}

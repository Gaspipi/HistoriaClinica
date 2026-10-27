using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HistoriaClinica.Core.Data;

/// <summary>
/// Interceptor que configura <c>PRAGMA busy_timeout</c> en cada conexión SQLite que abre EF Core,
/// para que las operaciones esperen ante bloqueos concurrentes en lugar de fallar de inmediato.
/// </summary>
public sealed class SqliteBusyTimeoutInterceptor : DbConnectionInterceptor
{
    private const string BusyTimeoutSql = "PRAGMA busy_timeout = 5000;";

    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result)
    {
        if (connection is SqliteConnection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = BusyTimeoutSql;
            command.ExecuteNonQuery();
        }

        return base.ConnectionOpening(connection, eventData, result);
    }

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        if (connection is SqliteConnection)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = BusyTimeoutSql;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
    }
}

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

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        Configurar(connection);
        base.ConnectionOpened(connection, eventData);
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        if (connection is SqliteConnection)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = BusyTimeoutSql;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    private static void Configurar(DbConnection connection)
    {
        if (connection is not SqliteConnection)
        {
            return;
        }

        using var command = connection.CreateCommand();
        command.CommandText = BusyTimeoutSql;
        command.ExecuteNonQuery();
    }
}

using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HistoriaClinica.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddHistoriaClinicaCore(
        this IServiceCollection services,
        string? databasePath = null)
    {
        if (databasePath is null)
        {
            services.TryAddSingleton<IDatabasePathProvider, DatabasePathProvider>();
        }
        else
        {
            services.AddSingleton<IDatabasePathProvider>(new DatabasePathProvider(databasePath));
        }

        services.AddSingleton<SqliteBusyTimeoutInterceptor>();

        services.AddDbContextFactory<HistoriaContext>((provider, options) =>
        {
            var path = provider.GetRequiredService<IDatabasePathProvider>().DatabasePath;
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = path,
                Mode = SqliteOpenMode.ReadWriteCreate,
                ForeignKeys = true,
            }.ToString();

            options.UseSqlite(connectionString);
            options.AddInterceptors(provider.GetRequiredService<SqliteBusyTimeoutInterceptor>());
        });

        services.AddSingleton<DatabaseInitializer>();
        services.AddSingleton<IPacienteRepository, PacienteRepository>();
        services.AddSingleton<IFichaRepository, FichaRepository>();
        services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
        services.AddSingleton<PacienteService>();
        services.AddSingleton<FichaService>();
        services.AddSingleton<AuthService>();

        return services;
    }
}

using HistoriaClinica.Core.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Tests;

public sealed class DatabaseInitializerTests : IDisposable
{
    private readonly string _databasePath;

    public DatabaseInitializerTests()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"historia-init-{Guid.NewGuid():N}.db");
    }

    [Fact]
    public async Task InicializaUnaBaseNuevaYCreaLaTablaDeUsuarios()
    {
        var factory = new TestDbContextFactory(_databasePath);
        var initializer = new DatabaseInitializer(factory, NullLogger<DatabaseInitializer>.Instance);

        await initializer.InitializeAsync();

        await using var context = factory.CreateDbContext();
        Assert.Equal(0, await context.Usuarios.CountAsync());
    }

    [Fact]
    public async Task AgregaUsuariosAUnaBaseExistenteSinPerderPacientes()
    {
        using (var connection = new SqliteConnection($"Data Source={_databasePath}"))
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE "Pacientes" (
                  "Dni" TEXT NOT NULL CONSTRAINT "PK_Pacientes" PRIMARY KEY,
                  "FirstName" TEXT NOT NULL,
                  "LastName" TEXT NOT NULL,
                  "DOB" TEXT NOT NULL,
                  "Phone" TEXT NOT NULL,
                  "ObraSocial" TEXT NOT NULL,
                  "NroSocio" TEXT NOT NULL,
                  "AntecFam" TEXT NOT NULL,
                  "AntecPers" TEXT NOT NULL,
                  "Medicacion" TEXT NOT NULL
                );
                INSERT INTO "Pacientes" (
                  "Dni", "FirstName", "LastName", "DOB", "Phone",
                  "ObraSocial", "NroSocio", "AntecFam", "AntecPers", "Medicacion"
                ) VALUES (
                  '12345678', 'Ana', 'Pérez', '1990-05-04 00:00:00',
                  '555-0000', 'OSDE', '123', '', '', ''
                );
                """;
            await command.ExecuteNonQueryAsync();
        }

        var factory = new TestDbContextFactory(_databasePath);
        var initializer = new DatabaseInitializer(factory, NullLogger<DatabaseInitializer>.Instance);

        await initializer.InitializeAsync();

        await using var context = factory.CreateDbContext();
        Assert.Equal(1, await context.Pacientes.CountAsync());
        Assert.Equal(0, await context.Usuarios.CountAsync());
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}

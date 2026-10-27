using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Tests;

public sealed class RepositoryTests : IDisposable
{
    private readonly string _databasePath;
    private readonly TestDbContextFactory _factory;
    private readonly PacienteRepository _pacientes;
    private readonly FichaRepository _fichas;

    public RepositoryTests()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"historia-test-{Guid.NewGuid():N}.db");
        _factory = new TestDbContextFactory(_databasePath);

        using (var context = _factory.CreateDbContext())
        {
            context.Database.EnsureCreated();
        }

        _pacientes = new PacienteRepository(_factory);
        _fichas = new FichaRepository(_factory);
    }

    [Fact]
    public async Task Paciente_SeGuardaYSeRecupera()
    {
        var paciente = CrearPaciente("12345678", "Ana", "Pérez");

        await _pacientes.SaveAsync(paciente);

        var recuperado = await _pacientes.GetByDniAsync("12345678");

        Assert.NotNull(recuperado);
        Assert.Equal("Ana", recuperado.FirstName);
        Assert.Equal(new DateOnly(1990, 5, 4), recuperado.DOB);
        Assert.Equal(new[] { "12345678" }, await _pacientes.GetDnisAsync());
    }

    [Fact]
    public async Task Paciente_SeActualizaSinDuplicar()
    {
        await _pacientes.SaveAsync(CrearPaciente("12345678", "Ana", "Pérez"));

        var actualizado = CrearPaciente("12345678", "Ana María", "Pérez");
        actualizado.Phone = "555-1234";
        await _pacientes.SaveAsync(actualizado);

        var recuperado = await _pacientes.GetByDniAsync("12345678");

        Assert.NotNull(recuperado);
        Assert.Equal("Ana María", recuperado.FirstName);
        Assert.Equal("555-1234", recuperado.Phone);
        Assert.Single(await _pacientes.GetDnisAsync());
    }

    [Fact]
    public async Task Ficha_SeEditaPorIdSinDuplicar()
    {
        await _pacientes.SaveAsync(CrearPaciente("12345678", "Ana", "Pérez"));
        var fecha = new DateTime(2026, 10, 22, 10, 30, 45, 123);
        var ficha = new FichaDiaria
        {
            Dni = "12345678",
            Fecha = fecha,
            Motivo = "Control",
            Enfermedad = "Sin particularidades",
            Indicaciones = "Reposo",
        };

        await _fichas.SaveAsync(ficha);
        var creada = await _fichas.GetByIdAsync(ficha.Id);

        Assert.NotNull(creada);
        Assert.True(ficha.Id > 0);

        creada.Motivo = "Control actualizado";
        creada.Indicaciones = "Reposo por 48 horas";
        await _fichas.SaveAsync(creada);

        var fichas = await _fichas.GetByDniAsync("12345678");
        var actualizada = Assert.Single(fichas);
        Assert.Equal("Control actualizado", actualizada.Motivo);
        Assert.Equal("Reposo por 48 horas", actualizada.Indicaciones);
        Assert.Equal(fecha, actualizada.Fecha);
    }

    [Fact]
    public async Task Ficha_SeListaOrdenadaPorFechaDescendente()
    {
        await _pacientes.SaveAsync(CrearPaciente("12345678", "Ana", "Pérez"));
        await _fichas.SaveAsync(CrearFicha("12345678", new DateTime(2026, 1, 1, 9, 0, 0)));
        await _fichas.SaveAsync(CrearFicha("12345678", new DateTime(2026, 3, 1, 9, 0, 0)));
        await _fichas.SaveAsync(CrearFicha("12345678", new DateTime(2026, 2, 1, 9, 0, 0)));

        var fichas = await _fichas.GetByDniAsync("12345678");

        Assert.Equal(
            new[] { new DateTime(2026, 3, 1, 9, 0, 0), new DateTime(2026, 2, 1, 9, 0, 0), new DateTime(2026, 1, 1, 9, 0, 0) },
            fichas.Select(f => f.Fecha));
    }

    [Fact]
    public async Task Ficha_SeBorraPorId()
    {
        await _pacientes.SaveAsync(CrearPaciente("12345678", "Ana", "Pérez"));
        var ficha = CrearFicha("12345678", DateTime.Now);
        await _fichas.SaveAsync(ficha);

        var borrada = await _fichas.DeleteAsync(ficha.Id);

        Assert.True(borrada);
        Assert.Empty(await _fichas.GetByDniAsync("12345678"));
        Assert.False(await _fichas.DeleteAsync(ficha.Id));
    }

    [Fact]
    public async Task Ficha_ConDniInexistente_FallaPorClaveForanea()
    {
        var ficha = CrearFicha("99999999", DateTime.Now);

        await Assert.ThrowsAsync<DbUpdateException>(() => _fichas.SaveAsync(ficha));
    }

    [Fact]
    public async Task Ficha_ConDniExistente_SeGuarda()
    {
        await _pacientes.SaveAsync(CrearPaciente("12345678", "Ana", "Pérez"));

        await _fichas.SaveAsync(CrearFicha("12345678", DateTime.Now));

        var fichas = await _fichas.GetByDniAsync("12345678");
        Assert.Single(fichas);
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }

    private static Paciente CrearPaciente(string dni, string nombre, string apellido) => new()
    {
        Dni = dni,
        FirstName = nombre,
        LastName = apellido,
        DOB = new DateOnly(1990, 5, 4),
        Phone = "555-0000",
        ObraSocial = "OSDE",
        NroSocio = "123",
        AntecFam = "Sin antecedentes",
        AntecPers = "Sin antecedentes",
        Medicacion = "Ninguna",
    };

    private static FichaDiaria CrearFicha(string dni, DateTime fecha) => new()
    {
        Dni = dni,
        Fecha = fecha,
        Motivo = "Control",
        Enfermedad = "Sin particularidades",
        Indicaciones = "Reposo",
    };

    private sealed class TestDbContextFactory : IDbContextFactory<HistoriaContext>
    {
        private readonly DbContextOptions<HistoriaContext> _options;

        public TestDbContextFactory(string databasePath)
        {
            _options = new DbContextOptionsBuilder<HistoriaContext>()
                .UseSqlite(new SqliteConnectionStringBuilder
                {
                    DataSource = databasePath,
                    ForeignKeys = true,
                }.ToString())
                .Options;
        }

        public HistoriaContext CreateDbContext() => new(_options);
    }
}

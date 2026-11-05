using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;

namespace HistoriaClinica.Tests;

public sealed class FichaRepositorySignosVitalesTests : IDisposable
{
    private readonly TestDatabase _database;
    private readonly PacienteRepository _pacientes;
    private readonly FichaRepository _fichas;

    public FichaRepositorySignosVitalesTests()
    {
        _database = new TestDatabase();
        _pacientes = new PacienteRepository(_database.Factory);
        _fichas = new FichaRepository(_database.Factory);
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Ficha_GuardaYRecuperaCerradaYSignosVitales()
    {
        await _pacientes.SaveAsync(CrearPaciente());
        var ficha = CrearFicha();
        ficha.Cerrada = true;
        ficha.PresionArterial = "120/80";
        ficha.FrecuenciaCardiaca = 72;
        ficha.Temperatura = 36.5m;
        ficha.Peso = 70.25m;

        await _fichas.SaveAsync(ficha);
        var recuperada = await _fichas.GetByIdAsync(ficha.Id);

        Assert.NotNull(recuperada);
        Assert.True(recuperada.Cerrada);
        Assert.Equal("120/80", recuperada.PresionArterial);
        Assert.Equal(72, recuperada.FrecuenciaCardiaca);
        Assert.Equal(36.5m, recuperada.Temperatura);
        Assert.Equal(70.25m, recuperada.Peso);
    }

    [Fact]
    public async Task Ficha_Nueva_UsaDefaults()
    {
        await _pacientes.SaveAsync(CrearPaciente());

        var ficha = CrearFicha();
        await _fichas.SaveAsync(ficha);
        var recuperada = await _fichas.GetByIdAsync(ficha.Id);

        Assert.NotNull(recuperada);
        Assert.False(recuperada.Cerrada);
        Assert.Null(recuperada.PresionArterial);
        Assert.Null(recuperada.FrecuenciaCardiaca);
        Assert.Null(recuperada.Temperatura);
        Assert.Null(recuperada.Peso);
    }

    [Fact]
    public async Task Ficha_ActualizaCerradaSinDuplicar()
    {
        await _pacientes.SaveAsync(CrearPaciente());
        var ficha = CrearFicha();
        await _fichas.SaveAsync(ficha);

        var recuperada = await _fichas.GetByIdAsync(ficha.Id);
        Assert.NotNull(recuperada);
        recuperada.Cerrada = true;
        await _fichas.SaveAsync(recuperada);

        var actualizada = await _fichas.GetByIdAsync(ficha.Id);
        Assert.NotNull(actualizada);
        Assert.True(actualizada.Cerrada);
        Assert.Single(await _fichas.GetByDniAsync("12345678"));
    }

    private static Paciente CrearPaciente() => new()
    {
        Dni = "12345678",
        Nombre = "Ana",
        Apellido = "Pérez",
    };

    private static FichaDiaria CrearFicha() => new()
    {
        Dni = "12345678",
        Fecha = new DateTime(2026, 10, 28, 9, 0, 0),
        Motivo = "Control",
        Enfermedad = "Sin particularidades",
        Indicaciones = "Reposo",
    };
}

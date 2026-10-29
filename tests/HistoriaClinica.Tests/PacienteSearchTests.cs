using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;

namespace HistoriaClinica.Tests;

public sealed class PacienteSearchTests : IDisposable
{
    private readonly TestDatabase _database;
    private readonly PacienteRepository _pacientes;

    public PacienteSearchTests()
    {
        _database = new TestDatabase();
        _pacientes = new PacienteRepository(_database.Factory);
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task SearchAsync_ConsultaVaciaOEspacios_DevuelveVacio()
    {
        await _pacientes.SaveAsync(CrearPaciente("12345678", "Ana", "Pérez"));

        Assert.Empty(await _pacientes.SearchAsync(string.Empty));
        Assert.Empty(await _pacientes.SearchAsync("   "));
        Assert.Empty(await _pacientes.SearchAsync(null!));
    }

    [Fact]
    public async Task SearchAsync_CoincidePorNombreYApellidoParcial()
    {
        await _pacientes.SaveAsync(CrearPaciente("12345678", "Ana", "Pérez"));
        await _pacientes.SaveAsync(CrearPaciente("23456789", "Beto", "Gómez"));

        var porNombre = await _pacientes.SearchAsync("An");
        var porApellido = await _pacientes.SearchAsync("ére");

        Assert.Equal(new[] { "12345678" }, porNombre.Select(paciente => paciente.Dni));
        Assert.Equal(new[] { "12345678" }, porApellido.Select(paciente => paciente.Dni));
    }

    [Fact]
    public async Task SearchAsync_OrdenaPorApellidoLuegoNombre()
    {
        await _pacientes.SaveAsync(CrearPaciente("30000001", "Carla", "Alvarez"));
        await _pacientes.SaveAsync(CrearPaciente("30000002", "Ana", "Bianchi"));
        await _pacientes.SaveAsync(CrearPaciente("30000003", "Beto", "Castro"));
        await _pacientes.SaveAsync(CrearPaciente("30000004", "Zoe", "Alvarez"));

        var resultados = await _pacientes.SearchAsync("a");

        Assert.Equal(
            new[] { "30000001", "30000004", "30000002", "30000003" },
            resultados.Select(paciente => paciente.Dni));
    }

    [Fact]
    public async Task SearchAsync_DevuelveCamposNuevosSinNavegacion()
    {
        await _pacientes.SaveAsync(new Paciente
        {
            Dni = "12345678",
            FirstName = "Ana",
            LastName = "Pérez",
            DOB = new DateOnly(1990, 5, 4),
            Alergias = "Penicilina",
            ProblemasActivos = "Asma",
        });

        var fichas = new FichaRepository(_database.Factory);
        await fichas.SaveAsync(new FichaDiaria
        {
            Dni = "12345678",
            Fecha = new DateTime(2026, 10, 28, 9, 0, 0),
            Motivo = "Control",
            Enfermedad = "Sin particularidades",
            Indicaciones = "Reposo",
        });

        var resultado = Assert.Single(await _pacientes.SearchAsync("Ana"));

        Assert.Equal(new DateOnly(1990, 5, 4), resultado.DOB);
        Assert.Equal("Penicilina", resultado.Alergias);
        Assert.Equal("Asma", resultado.ProblemasActivos);
        Assert.Empty(resultado.FichasDiarias);
    }

    [Fact]
    public async Task SearchAsync_SinCoincidencias_DevuelveVacio()
    {
        await _pacientes.SaveAsync(CrearPaciente("12345678", "Ana", "Pérez"));

        Assert.Empty(await _pacientes.SearchAsync("zzz"));
    }

    [Fact]
    public async Task SearchAsync_ConMinusculas_NoCoincidePorSerCaseSensitive()
    {
        await _pacientes.SaveAsync(CrearPaciente("12345678", "Ana", "Pérez"));

        var resultados = await _pacientes.SearchAsync("ana");

        Assert.Empty(resultados);
    }

    private static Paciente CrearPaciente(string dni, string nombre, string apellido) => new()
    {
        Dni = dni,
        FirstName = nombre,
        LastName = apellido,
    };
}

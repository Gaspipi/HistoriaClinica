using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Services;
using HistoriaClinica.Core.Validation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Tests;

public sealed class PlantillaServiceTests : IDisposable
{
    private readonly TestDatabase _database;
    private readonly PlantillaService _service;

    public PlantillaServiceTests()
    {
        _database = new TestDatabase();
        _service = new PlantillaService(new PlantillaRepository(_database.Factory), new PacienteRepository(_database.Factory));
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task SaveAsync_NormalizaYValida()
    {
        var plantilla = new Plantilla
        {
            Nombre = "  Control  ",
            Motivo = "  Motivo  ",
            Enfermedad = "  Enf  ",
            Indicaciones = "  Ind  ",
        };

        await _service.SaveAsync(plantilla);

        var recuperada = await _service.GetByIdAsync(plantilla.Id);
        Assert.NotNull(recuperada);
        Assert.Equal("Control", recuperada.Nombre);
        Assert.Equal("Motivo", recuperada.Motivo);
        Assert.Equal("Enf", recuperada.Enfermedad);
        Assert.Equal("Ind", recuperada.Indicaciones);
    }

    [Fact]
    public async Task SaveAsync_RechazaNombreVacio()
    {
        var plantilla = new Plantilla { Nombre = "   ", Motivo = "Motivo" };

        await Assert.ThrowsAsync<ValidationException>(() => _service.SaveAsync(plantilla));
    }

    [Fact]
    public async Task SaveAsync_RechazaMotivoVacio()
    {
        var plantilla = new Plantilla { Nombre = "Nombre", Motivo = "   " };

        await Assert.ThrowsAsync<ValidationException>(() => _service.SaveAsync(plantilla));
    }

    [Fact]
    public async Task DeleteAsync_PorIdYEntidad()
    {
        var plantilla = new Plantilla { Nombre = "Control", Motivo = "Motivo" };
        await _service.SaveAsync(plantilla);

        Assert.True(await _service.DeleteAsync(plantilla.Id));
        Assert.False(await _service.DeleteAsync(plantilla.Id));
    }

    [Fact]
    public async Task SaveAsync_NormalizaCreadorDni()
    {
        var pacientes = new PacienteRepository(_database.Factory);
        await pacientes.SaveAsync(new Paciente
        {
            Dni = "12345678",
            Nombre = "Ana",
            Apellido = "Pérez",
        });

        var conEspacios = new Plantilla
        {
            Nombre = "Control con creador",
            Motivo = "Motivo",
            CreadorDni = " 12345678 ",
        };
        await _service.SaveAsync(conEspacios);

        var recuperada = await _service.GetByIdAsync(conEspacios.Id);
        Assert.NotNull(recuperada);
        Assert.Equal("12345678", recuperada.CreadorDni);

        var vacio = new Plantilla
        {
            Nombre = "Control sin creador",
            Motivo = "Motivo",
            CreadorDni = "   ",
        };
        await _service.SaveAsync(vacio);

        var sinCreador = await _service.GetByIdAsync(vacio.Id);
        Assert.NotNull(sinCreador);
        Assert.Null(sinCreador.CreadorDni);
    }

    [Fact]
    public async Task SaveAsync_CreadorDniInvalido_LanzaValidationException()
    {
        var plantilla = new Plantilla
        {
            Nombre = "Control",
            Motivo = "Motivo",
            CreadorDni = "123",
        };

        var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.SaveAsync(plantilla));

        Assert.Contains(DniValidator.ErrorMessage, exception.Errors);
    }

    [Fact]
    public async Task DeleteAsync_PorEntidad_Elimina()
    {
        var plantilla = new Plantilla { Nombre = "Control", Motivo = "Motivo" };
        await _service.SaveAsync(plantilla);

        Assert.True(await _service.DeleteAsync(plantilla));
        Assert.Null(await _service.GetByIdAsync(plantilla.Id));
        Assert.False(await _service.DeleteAsync(plantilla));
    }
}

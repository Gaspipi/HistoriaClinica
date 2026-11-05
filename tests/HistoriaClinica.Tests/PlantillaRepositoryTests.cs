using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Tests;

public sealed class PlantillaRepositoryTests : IDisposable
{
    private readonly TestDatabase _database;
    private readonly PlantillaRepository _plantillas;

    public PlantillaRepositoryTests()
    {
        _database = new TestDatabase();
        _plantillas = new PlantillaRepository(_database.Factory);
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Plantilla_SeGuardaYSeRecupera()
    {
        var plantilla = new Plantilla
        {
            Nombre = "Control",
            Motivo = "Control de rutina",
            Enfermedad = "Sin particularidades",
            Indicaciones = "Reposo",
        };

        await _plantillas.SaveAsync(plantilla);

        Assert.True(plantilla.Id > 0);

        var recuperada = await _plantillas.GetByIdAsync(plantilla.Id);
        Assert.NotNull(recuperada);
        Assert.Equal("Control", recuperada.Nombre);
        Assert.Equal("Control de rutina", recuperada.Motivo);

        var todas = await _plantillas.GetAllAsync();
        Assert.Single(todas);
    }

    [Fact]
    public async Task Plantilla_SeActualizaSinDuplicar()
    {
        var plantilla = new Plantilla { Nombre = "Control", Motivo = "Motivo inicial" };
        await _plantillas.SaveAsync(plantilla);

        plantilla.Motivo = "Motivo actualizado";
        await _plantillas.SaveAsync(plantilla);

        var recuperada = await _plantillas.GetByIdAsync(plantilla.Id);
        Assert.NotNull(recuperada);
        Assert.Equal("Motivo actualizado", recuperada.Motivo);
        Assert.Single(await _plantillas.GetAllAsync());
    }

    [Fact]
    public async Task Plantilla_SeBorraPorId()
    {
        var plantilla = new Plantilla { Nombre = "Control", Motivo = "Motivo" };
        await _plantillas.SaveAsync(plantilla);

        var borrada = await _plantillas.DeleteAsync(plantilla.Id);

        Assert.True(borrada);
        Assert.Null(await _plantillas.GetByIdAsync(plantilla.Id));
        Assert.False(await _plantillas.DeleteAsync(plantilla.Id));
    }

    [Fact]
    public async Task Plantilla_SeBorraPorEntidad()
    {
        var plantilla = new Plantilla { Nombre = "Control", Motivo = "Motivo" };
        await _plantillas.SaveAsync(plantilla);

        var borrada = await _plantillas.DeleteAsync(plantilla);

        Assert.True(borrada);
    }
}

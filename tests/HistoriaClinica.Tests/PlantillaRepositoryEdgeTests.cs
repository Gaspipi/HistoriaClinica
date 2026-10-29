using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HistoriaClinica.Tests;

public sealed class PlantillaRepositoryEdgeTests : IDisposable
{
    private readonly TestDatabase _database;
    private readonly PlantillaRepository _plantillas;

    public PlantillaRepositoryEdgeTests()
    {
        _database = new TestDatabase();
        _plantillas = new PlantillaRepository(_database.Factory);
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Plantilla_NombreDuplicado_LanzaDbUpdateException()
    {
        await _plantillas.SaveAsync(new Plantilla { Nombre = "Control", Motivo = "Motivo" });

        await Assert.ThrowsAsync<DbUpdateException>(() => _plantillas.SaveAsync(
            new Plantilla { Nombre = "Control", Motivo = "Otro motivo" }));

        Assert.Single(await _plantillas.GetAllAsync());
    }

    [Fact]
    public async Task Plantilla_GetAll_OrdenaPorNombre()
    {
        await _plantillas.SaveAsync(new Plantilla { Nombre = "Zeta", Motivo = "M" });
        await _plantillas.SaveAsync(new Plantilla { Nombre = "Alfa", Motivo = "M" });
        await _plantillas.SaveAsync(new Plantilla { Nombre = "Media", Motivo = "M" });

        var plantillas = await _plantillas.GetAllAsync();

        Assert.Equal(
            new[] { "Alfa", "Media", "Zeta" },
            plantillas.Select(plantilla => plantilla.Nombre));
    }

    [Fact]
    public async Task Plantilla_ActualizarIdInexistente_LanzaInvalidOperationException()
    {
        var inexistente = new Plantilla { Id = 999, Nombre = "Control", Motivo = "Motivo" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _plantillas.SaveAsync(inexistente));
    }

    [Fact]
    public async Task Plantilla_Nueva_UsaDefaultsVacios()
    {
        var plantilla = new Plantilla { Nombre = "Control", Motivo = "Motivo" };

        await _plantillas.SaveAsync(plantilla);
        var recuperada = await _plantillas.GetByIdAsync(plantilla.Id);

        Assert.NotNull(recuperada);
        Assert.Equal(string.Empty, recuperada.Enfermedad);
        Assert.Equal(string.Empty, recuperada.Indicaciones);
        Assert.Null(recuperada.CreadorDni);
    }
}

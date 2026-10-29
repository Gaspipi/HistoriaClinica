using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Services;
using HistoriaClinica.App.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Tests;

public sealed class FichaEditorPlantillaTests : IDisposable
{
    private readonly TestDatabase _database;

    public FichaEditorPlantillaTests()
    {
        _database = new TestDatabase();
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Plantillas_SeCarganYSeAplican()
    {
        var plantillaRepo = new PlantillaRepository(_database.Factory);
        var plantillaService = new PlantillaService(plantillaRepo, new PacienteRepository(_database.Factory));
        await plantillaService.SaveAsync(new Plantilla
        {
            Nombre = "P1",
            Motivo = "Motivo plantilla",
            Enfermedad = "Enf plantilla",
            Indicaciones = "Ind plantilla",
        });

        var fichaRepo = new FakeFichaRepository();
        var fichaService = new FichaService(fichaRepo);
        var vm = new FichaEditorViewModel(
            fichaService,
            new FakeDialogService(),
            NullLogger<FichaEditorViewModel>.Instance,
            new FichaDiaria { Dni = "12345678", Fecha = new DateTime(2026, 10, 22, 10, 30, 0) },
            esNuevo: true,
            plantillaService: plantillaService);

        await vm.InitializeAsync();

        Assert.Single(vm.PlantillasDisponibles);
        var p = vm.PlantillasDisponibles[0];
        vm.PlantillaSeleccionada = p;
        vm.AplicarPlantillaCommand.Execute(null);

        Assert.Equal("Motivo plantilla", vm.Motivo);
        Assert.Equal("Enf plantilla", vm.Enfermedad);
        Assert.Equal("Ind plantilla", vm.Indicaciones);
    }
}

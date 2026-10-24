using HistoriaClinica.App.ViewModels;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Tests;

public sealed class EditorViewModelTests
{
    [Fact]
    public void Paciente_HayCambios_DetectaModificaciones()
    {
        var vm = CrearPacienteEditor();

        Assert.False(vm.HayCambios);
        vm.FirstName = "Ana";
        Assert.True(vm.HayCambios);
        vm.FirstName = string.Empty;
        Assert.False(vm.HayCambios);
    }

    [Fact]
    public async Task Paciente_ConDniInvalido_MuestraErrorYNoGuarda()
    {
        var pacientes = new FakePacienteRepository();
        var vm = CrearPacienteEditor(pacientes: pacientes, dni: string.Empty);

        await vm.GuardarCommand.ExecuteAsync(null);

        Assert.True(vm.TieneErrorDni);
        Assert.NotNull(vm.ErrorDni);
        Assert.Null(vm.Resultado);
        Assert.False(vm.CierreConfirmado);
        Assert.Empty(pacientes.Pacientes);
    }

    [Fact]
    public async Task Paciente_ConDatosValidos_GuardaYCierra()
    {
        var pacientes = new FakePacienteRepository();
        var vm = CrearPacienteEditor(pacientes: pacientes);
        vm.Dni = "12345678";
        vm.FirstName = "Ana";
        vm.LastName = "Pérez";
        vm.FechaNacimiento = new DateTime(1990, 5, 4);

        bool? resultadoCierre = null;
        vm.CloseRequested += (_, guardado) => resultadoCierre = guardado;

        await vm.GuardarCommand.ExecuteAsync(null);

        Assert.NotNull(vm.Resultado);
        Assert.Equal("Ana", vm.Resultado.FirstName);
        Assert.True(vm.CierreConfirmado);
        Assert.True(resultadoCierre);
        Assert.Single(pacientes.Pacientes);
    }

    [Fact]
    public async Task Paciente_ConFechaFutura_MuestraErrorDeCampo()
    {
        var vm = CrearPacienteEditor();
        vm.Dni = "12345678";
        vm.FechaNacimiento = DateTime.Today.AddDays(1);

        await vm.GuardarCommand.ExecuteAsync(null);

        Assert.True(vm.TieneErrorFechaNacimiento);
        Assert.Null(vm.Resultado);
        Assert.False(vm.HayError);
    }

    [Fact]
    public async Task Paciente_NuevoConDniDuplicado_RechazaGuardar()
    {
        var pacientes = new FakePacienteRepository();
        pacientes.Agregar(new Paciente { Dni = "12345678", FirstName = "Juan" });

        var vm = CrearPacienteEditor(pacientes: pacientes, dni: "12345678");

        await vm.GuardarCommand.ExecuteAsync(null);

        Assert.True(vm.TieneErrorDni);
        Assert.Contains("Ya existe", vm.ErrorDni);
        Assert.Null(vm.Resultado);
        Assert.False(vm.CierreConfirmado);
    }

    [Fact]
    public void Paciente_LimpiarFechaNacimiento_QuitaLaFecha()
    {
        var vm = CrearPacienteEditor();
        vm.FechaNacimiento = new DateTime(1995, 3, 10);
        Assert.True(vm.TieneFechaNacimiento);

        vm.LimpiarFechaNacimientoCommand.Execute(null);

        Assert.False(vm.TieneFechaNacimiento);
        Assert.Null(vm.FechaNacimiento);
    }

    [Fact]
    public async Task Cancelar_SinCambios_NoPideConfirmacion()
    {
        var dialogos = new FakeDialogService();
        var vm = CrearPacienteEditor(dialogos: dialogos);

        await vm.CancelarCommand.ExecuteAsync(null);

        Assert.Equal(0, dialogos.Confirmaciones);
        Assert.True(vm.CierreConfirmado);
    }

    [Fact]
    public async Task Cancelar_ConCambios_PideConfirmacion()
    {
        var dialogos = new FakeDialogService { ConfirmarResultado = false };
        var vm = CrearPacienteEditor(dialogos: dialogos);
        vm.FirstName = "Ana";

        await vm.CancelarCommand.ExecuteAsync(null);

        Assert.Equal(1, dialogos.Confirmaciones);
        Assert.False(vm.CierreConfirmado);

        dialogos.ConfirmarResultado = true;
        await vm.CancelarCommand.ExecuteAsync(null);

        Assert.Equal(2, dialogos.Confirmaciones);
        Assert.True(vm.CierreConfirmado);
    }

    [Fact]
    public async Task Ficha_SinCamposObligatorios_MuestraErroresYNoGuarda()
    {
        var fichas = new FakeFichaRepository();
        var vm = CrearFichaEditor(fichas: fichas);

        await vm.GuardarCommand.ExecuteAsync(null);

        Assert.True(vm.TieneErrorMotivo);
        Assert.True(vm.TieneErrorEnfermedad);
        Assert.True(vm.TieneErrorIndicaciones);
        Assert.Null(vm.Resultado);
        Assert.False(vm.CierreConfirmado);
        Assert.Empty(fichas.Fichas);
        Assert.False(vm.HayError);
    }

    [Fact]
    public async Task Ficha_ConDatosValidos_GuardaYCierra()
    {
        var fichas = new FakeFichaRepository();
        var vm = CrearFichaEditor(fichas: fichas);
        vm.Motivo = "Control";
        vm.Enfermedad = "Sin particularidades";
        vm.Indicaciones = "Reposo";

        await vm.GuardarCommand.ExecuteAsync(null);

        Assert.NotNull(vm.Resultado);
        Assert.True(vm.Resultado.Id > 0);
        Assert.Single(fichas.Fichas);
        Assert.True(vm.CierreConfirmado);
    }

    [Fact]
    public void Ficha_HayCambios_DetectaModificaciones()
    {
        var vm = CrearFichaEditor();

        Assert.False(vm.HayCambios);
        vm.Motivo = "Control";
        Assert.True(vm.HayCambios);
    }

    private static PatientEditorViewModel CrearPacienteEditor(
        FakePacienteRepository? pacientes = null,
        FakeDialogService? dialogos = null,
        string dni = "12345678")
    {
        pacientes ??= new FakePacienteRepository();
        dialogos ??= new FakeDialogService();

        return new PatientEditorViewModel(
            new PacienteService(pacientes),
            dialogos,
            NullLogger<PatientEditorViewModel>.Instance,
            new Paciente { Dni = dni },
            esNuevo: true);
    }

    private static FichaEditorViewModel CrearFichaEditor(
        FakeFichaRepository? fichas = null,
        FakeDialogService? dialogos = null)
    {
        fichas ??= new FakeFichaRepository();
        dialogos ??= new FakeDialogService();

        return new FichaEditorViewModel(
            new FichaService(fichas),
            dialogos,
            NullLogger<FichaEditorViewModel>.Instance,
            new FichaDiaria { Dni = "12345678", Fecha = new DateTime(2026, 10, 22, 10, 30, 0) },
            esNuevo: true);
    }
}

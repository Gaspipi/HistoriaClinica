using HistoriaClinica.App.Services;
using HistoriaClinica.App.ViewModels;
using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Services;
using HistoriaClinica.Core.Validation;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Tests;

public sealed class MainWindowViewModelTests
{
    [Fact]
    public void PacienteActual_ExponeDatosParaLaUi()
    {
        var contexto = CrearViewModel();
        var paciente = CrearPaciente("12345678", "Ana", "Pérez");
        paciente.DOB = DateTime.Today.AddYears(-30).AddDays(-1);
        paciente.ObraSocial = "OSDE";

        contexto.Vm.PacienteActual = paciente;

        Assert.True(contexto.Vm.HayPaciente);
        Assert.True(contexto.Vm.HayFichaSeleccionada == false);
        Assert.Equal("Ana Pérez", contexto.Vm.NombreCompleto);
        Assert.Equal("AP", contexto.Vm.InicialesPaciente);
        Assert.Equal("DNI 12345678", contexto.Vm.DniPacienteTexto);
        Assert.Equal("30 años", contexto.Vm.EdadTexto);
        Assert.Equal("OSDE", contexto.Vm.ObraSocialTexto);
        Assert.Contains("DNI 12345678", contexto.Vm.PacienteResumen);
        Assert.Contains("30 años", contexto.Vm.PacienteResumen);
        Assert.Equal("AN", contexto.Vm.InicialesUsuario);
    }

    [Fact]
    public void PacienteActual_Vacio_MuestraValoresPredeterminados()
    {
        var contexto = CrearViewModel();

        Assert.False(contexto.Vm.HayPaciente);
        Assert.Equal("Paciente sin nombre", contexto.Vm.NombreCompleto);
        Assert.Equal("?", contexto.Vm.InicialesPaciente);
        Assert.Equal("Sin DNI", contexto.Vm.DniPacienteTexto);
        Assert.Equal("Sin fecha de nacimiento", contexto.Vm.EdadTexto);
        Assert.Equal("Sin obra social", contexto.Vm.ObraSocialTexto);
    }

    [Fact]
    public void Fichas_ActualizanIndicadoresDeLaLista()
    {
        var contexto = CrearViewModel();

        Assert.False(contexto.Vm.HayFichas);
        Assert.Equal("Sin fichas", contexto.Vm.CantidadFichasTexto);

        contexto.Vm.Fichas.Add(new FichaDiaria { Id = 1, Dni = "12345678", Motivo = "Control" });
        Assert.True(contexto.Vm.HayFichas);
        Assert.Equal("1 ficha", contexto.Vm.CantidadFichasTexto);

        contexto.Vm.Fichas.Add(new FichaDiaria { Id = 2, Dni = "12345678", Motivo = "Control" });
        Assert.Equal("2 fichas", contexto.Vm.CantidadFichasTexto);

        contexto.Vm.Fichas.Clear();
        Assert.False(contexto.Vm.HayFichas);
        Assert.Equal("Sin fichas", contexto.Vm.CantidadFichasTexto);
    }

    [Fact]
    public async Task BuscarConDniInvalido_MuestraErrorDeValidacion()
    {
        var contexto = CrearViewModel();
        contexto.Vm.Dni = "123";

        await contexto.Vm.BuscarCommand.ExecuteAsync(null);

        Assert.True(contexto.Vm.HayError);
        Assert.Equal(DniValidator.ErrorMessage, contexto.Vm.MensajeError);
        Assert.False(contexto.Vm.IsBusy);
        Assert.Null(contexto.Vm.PacienteActual);
    }

    [Fact]
    public async Task NuevoPaciente_ConDialogo_ActualizaPacienteYEstado()
    {
        var contexto = CrearViewModel();
        var paciente = CrearPaciente("12345678", "Ana", "Pérez");
        contexto.Pacientes.Agregar(paciente);
        contexto.Dialogos.PacienteResultado = paciente;

        await contexto.Vm.NuevoPacienteCommand.ExecuteAsync(null);

        Assert.Same(paciente, contexto.Vm.PacienteActual);
        Assert.True(contexto.Vm.HayPaciente);
        Assert.Equal("Paciente creado.", contexto.Vm.MensajeEstado);
        Assert.True(contexto.Vm.HayMensajeEstado);
        Assert.True(contexto.Dialogos.EsNuevoSolicitado);
        Assert.Empty(contexto.Dialogos.PacienteSolicitado!.Dni);
    }

    [Fact]
    public async Task BorrarFicha_ConConfirmacion_RecargaYNotifica()
    {
        var contexto = CrearViewModel();
        var paciente = CrearPaciente("12345678", "Ana", "Pérez");
        contexto.Pacientes.Agregar(paciente);
        var ficha = contexto.Fichas.Agregar(new FichaDiaria
        {
            Dni = paciente.Dni,
            Fecha = DateTime.Today,
            Motivo = "Control",
            Enfermedad = "Sin particularidades",
            Indicaciones = "Reposo",
        });

        contexto.Vm.PacienteActual = paciente;
        contexto.Vm.Fichas.Add(ficha);
        contexto.Vm.FichaSeleccionada = ficha;
        contexto.Dialogos.ConfirmarResultado = true;

        await contexto.Vm.BorrarFichaCommand.ExecuteAsync(null);

        Assert.Equal(1, contexto.Dialogos.Confirmaciones);
        Assert.Contains("Motivo: Control", contexto.Dialogos.UltimoMensajeConfirmacion);
        Assert.Empty(contexto.Fichas.Fichas);
        Assert.False(contexto.Vm.HayFichas);
        Assert.Null(contexto.Vm.FichaSeleccionada);
        Assert.Equal("Ficha borrada.", contexto.Vm.MensajeEstado);
    }

    [Fact]
    public async Task BorrarFicha_Cancelado_NoBorra()
    {
        var contexto = CrearViewModel();
        var paciente = CrearPaciente("12345678", "Ana", "Pérez");
        contexto.Pacientes.Agregar(paciente);
        var ficha = contexto.Fichas.Agregar(new FichaDiaria
        {
            Dni = paciente.Dni,
            Fecha = DateTime.Today,
            Motivo = "Control",
            Enfermedad = "Sin particularidades",
            Indicaciones = "Reposo",
        });

        contexto.Vm.PacienteActual = paciente;
        contexto.Vm.FichaSeleccionada = ficha;
        contexto.Dialogos.ConfirmarResultado = false;

        await contexto.Vm.BorrarFichaCommand.ExecuteAsync(null);

        Assert.Single(contexto.Fichas.Fichas);
        Assert.Null(contexto.Vm.MensajeEstado);
    }

    [Fact]
    public async Task CopiarDatos_UsaElPortapapeles()
    {
        var contexto = CrearViewModel();
        contexto.Vm.PacienteActual = CrearPaciente("12345678", "Ana", "Pérez");

        await contexto.Vm.CopiarResumenCommand.ExecuteAsync(null);
        Assert.Contains("Ana Pérez", contexto.Clipboard.Texto);
        Assert.Contains("DNI 12345678", contexto.Clipboard.Texto);

        await contexto.Vm.CopiarDniCommand.ExecuteAsync(null);
        Assert.Equal("12345678", contexto.Clipboard.Texto);

        await contexto.Vm.CopiarTelefonoCommand.ExecuteAsync(null);
        Assert.Equal("555-0000", contexto.Clipboard.Texto);
    }

    [Fact]
    public async Task Mantenimiento_AbreCarpetaYCreaCopia()
    {
        var contexto = CrearViewModel();
        contexto.Maintenance.RutaCopia = "/tmp/Backups/Historia-20261023-120000.db";

        await contexto.Vm.AbrirCarpetaDatosCommand.ExecuteAsync(null);
        Assert.True(contexto.Maintenance.CarpetaAbierta);

        await contexto.Vm.CrearCopiaSeguridadCommand.ExecuteAsync(null);
        Assert.True(contexto.Maintenance.CopiaCreada);
        Assert.Contains("Historia-20261023-120000.db", contexto.Vm.MensajeEstado);
    }

    [Fact]
    public async Task Preferencias_AbrenElDialogo()
    {
        var contexto = CrearViewModel();

        await contexto.Vm.MostrarPreferenciasCommand.ExecuteAsync(null);

        Assert.Equal(1, contexto.Dialogos.PreferenciasMostradas);
    }

    [Fact]
    public void CerrarSesion_LimpiaEstadoYNavegaAlLogin()
    {
        var contexto = CrearViewModel();
        contexto.Vm.PacienteActual = CrearPaciente("12345678", "Ana", "Pérez");
        contexto.Vm.Fichas.Add(new FichaDiaria { Id = 1, Dni = "12345678" });
        contexto.Vm.Dni = "12345678";

        contexto.Vm.CerrarSesionCommand.Execute(null);

        Assert.False(contexto.Sesion.EstaAutenticada);
        Assert.Null(contexto.Vm.PacienteActual);
        Assert.Empty(contexto.Vm.Fichas);
        Assert.Equal(string.Empty, contexto.Vm.Dni);
        Assert.Equal(1, contexto.Navigator.VecesAuth);
    }

    [Fact]
    public void ComandosDeLimpieza_BorranMensajes()
    {
        var contexto = CrearViewModel();
        contexto.Vm.MensajeError = "Error";
        contexto.Vm.MensajeEstado = "Estado";

        contexto.Vm.LimpiarErrorCommand.Execute(null);
        contexto.Vm.LimpiarMensajeCommand.Execute(null);

        Assert.False(contexto.Vm.HayError);
        Assert.False(contexto.Vm.HayMensajeEstado);
    }

    [Fact]
    public async Task AcercaDe_IncluyeAtajosYVersion()
    {
        var contexto = CrearViewModel();
        await contexto.Vm.MostrarAcercaDeCommand.ExecuteAsync(null);

        Assert.Single(contexto.Dialogos.MensajesInfo);
        var dialogo = contexto.Dialogos.MensajesInfo[0];
        Assert.Equal("Acerca de", dialogo.Titulo);
        Assert.Contains("Ctrl+E", dialogo.Mensaje);
        Assert.Contains("Ctrl+N", dialogo.Mensaje);
        Assert.Contains("Ctrl+Shift+N", dialogo.Mensaje);
        Assert.Contains("Esc:", dialogo.Mensaje);
    }

    private static Contexto CrearViewModel()
    {
        var pacientes = new FakePacienteRepository();
        var fichas = new FakeFichaRepository();
        var dialogos = new FakeDialogService();
        var session = new UserSession();
        session.Iniciar(new Usuario { Id = 1, Username = "ana" });
        var navigator = new FakeAppNavigator();
        var clipboard = new FakeClipboardService();
        var maintenance = new FakeDataMaintenanceService();

        var vm = new MainWindowViewModel(
            new PacienteService(pacientes),
            new FichaService(fichas),
            dialogos,
            session,
            navigator,
            clipboard,
            maintenance,
            NullLogger<MainWindowViewModel>.Instance);

        return new Contexto(vm, pacientes, fichas, dialogos, session, navigator, clipboard, maintenance);
    }

    private static Paciente CrearPaciente(string dni, string nombre, string apellido) => new()
    {
        Dni = dni,
        FirstName = nombre,
        LastName = apellido,
        DOB = new DateTime(1990, 5, 4),
        Phone = "555-0000",
        ObraSocial = "OSDE",
        NroSocio = "123",
        AntecFam = "Sin antecedentes",
        AntecPers = "Sin antecedentes",
        Medicacion = "Ninguna",
    };

    private sealed record Contexto(
        MainWindowViewModel Vm,
        FakePacienteRepository Pacientes,
        FakeFichaRepository Fichas,
        FakeDialogService Dialogos,
        UserSession Sesion,
        FakeAppNavigator Navigator,
        FakeClipboardService Clipboard,
        FakeDataMaintenanceService Maintenance);
}

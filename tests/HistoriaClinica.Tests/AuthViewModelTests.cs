using HistoriaClinica.App.Services;
using HistoriaClinica.App.ViewModels;
using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Repositories;
using HistoriaClinica.Core.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace HistoriaClinica.Tests;

public sealed class AuthViewModelTests : IDisposable
{
    private readonly TestDatabase _database;
    private readonly AuthService _auth;
    private readonly DatabaseInitializer _initializer;

    public AuthViewModelTests()
    {
        _database = new TestDatabase();
        _auth = new AuthService(new UsuarioRepository(_database.Factory));
        _initializer = new DatabaseInitializer(
            _database.Factory,
            NullLogger<DatabaseInitializer>.Instance);
    }

    [Fact]
    public async Task PrimeraEjecucion_MuestraRegistro()
    {
        var (vm, _, _) = CrearViewModel();

        await vm.InitializeAsync();

        Assert.True(vm.BaseLista);
        Assert.True(vm.EsRegistro);
        Assert.True(vm.HayFormulario);
        Assert.False(vm.IsInicializando);
    }

    [Fact]
    public void MostrarPassword_CambiaElTextoDelBoton()
    {
        var (vm, _, _) = CrearViewModel();

        Assert.Equal("Mostrar", vm.TextoMostrarPassword);

        vm.MostrarPassword = true;

        Assert.Equal("Ocultar", vm.TextoMostrarPassword);
    }

    [Fact]
    public async Task Registro_CreaUsuarioEIngresaDirecto()
    {
        var (vm, session, navigator) = CrearViewModel();
        await vm.InitializeAsync();
        vm.Usuario = "ana";
        vm.Password = "clave-segura-123";
        vm.ConfirmacionPassword = "clave-segura-123";

        await vm.RegistrarCommand.ExecuteAsync(null);

        Assert.True(session.EstaAutenticada);
        Assert.Equal("ana", session.NombreUsuario);
        Assert.Equal(1, navigator.VecesMain);
        Assert.False(vm.HayError);
    }

    [Fact]
    public async Task Registro_ConPasswordsDistintas_MuestraError()
    {
        var (vm, session, navigator) = CrearViewModel();
        await vm.InitializeAsync();
        vm.Usuario = "ana";
        vm.Password = "clave-segura-123";
        vm.ConfirmacionPassword = "otra-clave";

        await vm.RegistrarCommand.ExecuteAsync(null);

        Assert.True(vm.TieneErrorConfirmacion);
        Assert.False(session.EstaAutenticada);
        Assert.Equal(0, navigator.VecesMain);
    }

    [Fact]
    public async Task SegundoArranque_MuestraLoginYRechazaCredencialesInvalidas()
    {
        await _auth.RegisterFirstUserAsync("ana", "clave-segura-123");
        var (vm, session, navigator) = CrearViewModel();

        await vm.InitializeAsync();
        Assert.False(vm.EsRegistro);

        vm.Usuario = "ana";
        vm.Password = "incorrecta";
        await vm.IngresarCommand.ExecuteAsync(null);

        Assert.False(session.EstaAutenticada);
        Assert.Equal(0, navigator.VecesMain);
        Assert.Equal("Usuario o contraseña incorrectos.", vm.MensajeError);
    }

    [Fact]
    public async Task LoginCorrecto_IngresaALaAplicacion()
    {
        await _auth.RegisterFirstUserAsync("ana", "clave-segura-123");
        var (vm, session, navigator) = CrearViewModel();
        await vm.InitializeAsync();
        vm.Usuario = "ANA";
        vm.Password = "clave-segura-123";

        await vm.IngresarCommand.ExecuteAsync(null);

        Assert.True(session.EstaAutenticada);
        Assert.Equal(1, navigator.VecesMain);
    }

    public void Dispose() => _database.Dispose();

    private (AuthViewModel Vm, UserSession Session, FakeAppNavigator Navigator) CrearViewModel()
    {
        var session = new UserSession();
        var navigator = new FakeAppNavigator();
        var vm = new AuthViewModel(
            _initializer,
            _auth,
            session,
            navigator,
            NullLogger<AuthViewModel>.Instance);

        return (vm, session, navigator);
    }
}

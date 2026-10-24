using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HistoriaClinica.App.Services;
using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Services;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    private readonly DatabaseInitializer _databaseInitializer;
    private readonly AuthService _authService;
    private readonly UserSession _session;
    private readonly IAppNavigator _navigator;
    private readonly ILogger<AuthViewModel> _logger;
    private bool _inicializacionEnCurso;

    public AuthViewModel(
        DatabaseInitializer databaseInitializer,
        AuthService authService,
        UserSession session,
        IAppNavigator navigator,
        ILogger<AuthViewModel> logger)
    {
        _databaseInitializer = databaseInitializer;
        _authService = authService;
        _session = session;
        _navigator = navigator;
        _logger = logger;
        IsInicializando = true;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayFormulario))]
    [NotifyPropertyChangedFor(nameof(MostrarReintento))]
    [NotifyPropertyChangedFor(nameof(MostrarErrorFatal))]
    [NotifyCanExecuteChangedFor(nameof(IngresarCommand))]
    [NotifyCanExecuteChangedFor(nameof(RegistrarCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReintentarCommand))]
    private bool _isInicializando;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(IngresarCommand))]
    [NotifyCanExecuteChangedFor(nameof(RegistrarCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReintentarCommand))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayFormulario))]
    [NotifyPropertyChangedFor(nameof(MostrarReintento))]
    [NotifyPropertyChangedFor(nameof(Titulo))]
    [NotifyPropertyChangedFor(nameof(Subtitulo))]
    [NotifyCanExecuteChangedFor(nameof(IngresarCommand))]
    [NotifyCanExecuteChangedFor(nameof(RegistrarCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReintentarCommand))]
    private bool _baseLista;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Titulo))]
    [NotifyPropertyChangedFor(nameof(Subtitulo))]
    private bool _esRegistro;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarReintento))]
    [NotifyPropertyChangedFor(nameof(MostrarErrorFatal))]
    [NotifyCanExecuteChangedFor(nameof(ReintentarCommand))]
    private bool _errorFatal;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayError))]
    private string? _mensajeError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneErrorUsuario))]
    private string? _errorUsuario;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneErrorPassword))]
    private string? _errorPassword;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneErrorConfirmacion))]
    private string? _errorConfirmacion;

    [ObservableProperty]
    private string _usuario = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmacionPassword = string.Empty;

    [ObservableProperty]
    private bool _mostrarPassword;

    public bool HayError => !string.IsNullOrWhiteSpace(MensajeError);

    public bool HayFormulario => BaseLista && !IsInicializando;

    public bool MostrarReintento => !BaseLista && !IsInicializando && !ErrorFatal;

    public bool MostrarErrorFatal => ErrorFatal && !IsInicializando;

    public bool TieneErrorUsuario => !string.IsNullOrWhiteSpace(ErrorUsuario);

    public bool TieneErrorPassword => !string.IsNullOrWhiteSpace(ErrorPassword);

    public bool TieneErrorConfirmacion => !string.IsNullOrWhiteSpace(ErrorConfirmacion);

    public string TextoMostrarPassword => MostrarPassword ? "Ocultar" : "Mostrar";

    public string Titulo => EsRegistro
        ? "Creá el primer usuario"
        : "Ingresá a tu cuenta";

    public string Subtitulo => EsRegistro
        ? "Configurá el acceso inicial para empezar a usar Historia Clínica."
        : "Usá tus credenciales para continuar.";

    public async Task InitializeAsync()
    {
        if (_inicializacionEnCurso)
        {
            return;
        }

        _inicializacionEnCurso = true;
        IsInicializando = true;
        BaseLista = false;
        ErrorFatal = false;
        MensajeError = null;

        try
        {
            await _databaseInitializer.InitializeAsync();
            EsRegistro = !await _authService.HasUsersAsync();
            BaseLista = true;
        }
        catch (DatabaseSchemaVersionException ex)
        {
            _logger.LogError(ex, "La base de datos fue creada por una versión más nueva");
            ErrorFatal = true;
            MensajeError = ex.Message;
        }
        catch (DatabaseMigrationException ex)
        {
            _logger.LogError(ex, "Error al migrar la base de datos");
            MensajeError = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar la autenticación");
            MensajeError = "No se pudo preparar la base de datos. Verificá la ruta e intentá nuevamente.";
        }
        finally
        {
            IsInicializando = false;
            _inicializacionEnCurso = false;
        }
    }

    [RelayCommand(CanExecute = nameof(PuedeAutenticar))]
    private async Task IngresarAsync()
    {
        if (IsBusy)
        {
            return;
        }

        LimpiarErrores();

        if (string.IsNullOrWhiteSpace(Usuario))
        {
            ErrorUsuario = "Ingresá tu usuario.";
            return;
        }

        if (string.IsNullOrEmpty(Password))
        {
            ErrorPassword = "Ingresá tu contraseña.";
            return;
        }

        try
        {
            IsBusy = true;
            var usuario = await _authService.LoginAsync(Usuario, Password);

            if (usuario is null)
            {
                MensajeError = "Usuario o contraseña incorrectos.";
                Password = string.Empty;
                return;
            }

            _session.Iniciar(usuario);
            Password = string.Empty;
            _navigator.ShowMain();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar sesión");
            MensajeError = "No se pudo iniciar sesión. Intentá nuevamente.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(PuedeAutenticar))]
    private async Task RegistrarAsync()
    {
        if (IsBusy)
        {
            return;
        }

        LimpiarErrores();

        ErrorUsuario = AuthService.ObtenerErrorUsuario(Usuario);
        ErrorPassword = AuthService.ObtenerErrorPassword(Password);

        if (Password != ConfirmacionPassword)
        {
            ErrorConfirmacion = "Las contraseñas no coinciden.";
        }

        if (TieneErrorUsuario || TieneErrorPassword || TieneErrorConfirmacion)
        {
            return;
        }

        try
        {
            IsBusy = true;
            var usuario = await _authService.RegisterFirstUserAsync(Usuario, Password);
            _session.Iniciar(usuario);
            Password = string.Empty;
            ConfirmacionPassword = string.Empty;
            _navigator.ShowMain();
        }
        catch (ValidationException ex)
        {
            MensajeError = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar el primer usuario");
            MensajeError = "No se pudo crear el usuario. Intentá nuevamente.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(PuedeReintentar))]
    private Task ReintentarAsync() => InitializeAsync();

    private bool PuedeAutenticar() => BaseLista && !IsInicializando && !IsBusy;

    private bool PuedeReintentar() => !BaseLista && !IsInicializando && !IsBusy && !ErrorFatal;

    private void LimpiarErrores()
    {
        MensajeError = null;
        ErrorUsuario = null;
        ErrorPassword = null;
        ErrorConfirmacion = null;
    }

    partial void OnUsuarioChanged(string value) => ErrorUsuario = null;

    partial void OnPasswordChanged(string value) => ErrorPassword = null;

    partial void OnConfirmacionPasswordChanged(string value) => ErrorConfirmacion = null;

    partial void OnMostrarPasswordChanged(bool value) =>
        OnPropertyChanged(nameof(TextoMostrarPassword));
}

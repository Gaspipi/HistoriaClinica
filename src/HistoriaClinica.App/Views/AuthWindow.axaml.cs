using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Threading;
using HistoriaClinica.App.Layout;
using HistoriaClinica.App.ViewModels;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.Views;

public partial class AuthWindow : Window
{
    private readonly AuthViewModel _viewModel;
    private readonly ILogger<AuthWindow> _logger;
    private readonly TextBox _usuarioTextBox;
    private readonly TextBox _registroUsuarioTextBox;

    public AuthWindow(AuthViewModel viewModel, ILogger<AuthWindow> logger)
    {
        InitializeComponent();

        Responsive.Attach(this, compactWidth: 480, narrowWidth: 400, shortHeight: 500);

        _viewModel = viewModel;
        _logger = logger;
        DataContext = viewModel;
        Icon = AppIcons.Load();

        _usuarioTextBox = this.FindControl<TextBox>("UsuarioTextBox")
            ?? throw new InvalidOperationException("No se encontró el control UsuarioTextBox");
        _registroUsuarioTextBox = this.FindControl<TextBox>("RegistroUsuarioTextBox")
            ?? throw new InvalidOperationException("No se encontró el control RegistroUsuarioTextBox");

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        try
        {
            WindowSizing.AjustarAlAreaDeTrabajo(this);
            await _viewModel.InitializeAsync();
            EnfocarUsuario();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al abrir la ventana de autenticación");
            _viewModel.MensajeError = "No se pudo abrir la ventana de autenticación. Intentá nuevamente.";
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if ((e.PropertyName is nameof(AuthViewModel.HayFormulario) or nameof(AuthViewModel.EsRegistro))
            && _viewModel.HayFormulario)
        {
            Dispatcher.UIThread.Post(EnfocarUsuario);
        }
    }

    private void EnfocarUsuario()
    {
        var campo = _viewModel.EsRegistro ? _registroUsuarioTextBox : _usuarioTextBox;
        campo.Focus();
        campo.SelectAll();
    }

    protected override void OnClosed(EventArgs e)
    {
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        base.OnClosed(e);
    }
}

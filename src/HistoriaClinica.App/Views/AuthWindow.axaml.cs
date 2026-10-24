using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Threading;
using HistoriaClinica.App.Layout;
using HistoriaClinica.App.ViewModels;

namespace HistoriaClinica.App.Views;

public partial class AuthWindow : Window
{
    private readonly AuthViewModel _viewModel;
    private readonly TextBox _usuarioTextBox;
    private readonly TextBox _registroUsuarioTextBox;

    public AuthWindow(AuthViewModel viewModel)
    {
        InitializeComponent();

        Responsive.Attach(this, compactWidth: 480, narrowWidth: 400, shortHeight: 500);

        _viewModel = viewModel;
        DataContext = viewModel;
        Icon = AppIcons.Load();

        _usuarioTextBox = this.FindControl<TextBox>("UsuarioTextBox")!;
        _registroUsuarioTextBox = this.FindControl<TextBox>("RegistroUsuarioTextBox")!;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        WindowSizing.AjustarAlAreaDeTrabajo(this);
        await _viewModel.InitializeAsync();
        EnfocarUsuario();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AuthViewModel.HayFormulario) or nameof(AuthViewModel.EsRegistro)
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

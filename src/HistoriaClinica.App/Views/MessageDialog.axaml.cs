using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using HistoriaClinica.App.Layout;

namespace HistoriaClinica.App.Views;

public partial class MessageDialog : Window
{
    private readonly TextBlock _titleText;
    private readonly TextBlock _messageText;
    private readonly TextBlock _acceptText;
    private readonly TextBlock _cancelText;
    private readonly Button _acceptButton;
    private readonly Button _cancelButton;
    private readonly bool _esDestructiva;

    public MessageDialog(
        string titulo,
        string mensaje,
        bool mostrarCancelar,
        string textoAceptar = "Aceptar",
        string textoCancelar = "Cancelar",
        bool esDestructiva = false)
    {
        InitializeComponent();

        Responsive.Attach(this, compactWidth: 400, narrowWidth: 320, shortHeight: 500);

        _titleText = this.FindControl<TextBlock>("TitleText")
            ?? throw new InvalidOperationException("No se encontró el control TitleText");
        _messageText = this.FindControl<TextBlock>("MessageText")
            ?? throw new InvalidOperationException("No se encontró el control MessageText");
        _acceptText = this.FindControl<TextBlock>("AcceptText")
            ?? throw new InvalidOperationException("No se encontró el control AcceptText");
        _cancelText = this.FindControl<TextBlock>("CancelText")
            ?? throw new InvalidOperationException("No se encontró el control CancelText");
        _acceptButton = this.FindControl<Button>("AcceptButton")
            ?? throw new InvalidOperationException("No se encontró el control AcceptButton");
        _cancelButton = this.FindControl<Button>("CancelButton")
            ?? throw new InvalidOperationException("No se encontró el control CancelButton");
        _esDestructiva = esDestructiva;

        Title = titulo;
        Icon = AppIcons.Load();
        _titleText.Text = titulo;
        _titleText.IsVisible = !string.IsNullOrWhiteSpace(titulo);
        _messageText.Text = mensaje;
        _acceptText.Text = textoAceptar;
        _cancelText.Text = textoCancelar;
        AutomationProperties.SetName(_acceptButton, textoAceptar);
        AutomationProperties.SetName(_cancelButton, textoCancelar);
        _cancelButton.IsVisible = mostrarCancelar;

        _acceptButton.Click += OnAcceptClick;
        _cancelButton.Click += OnCancelClick;
        KeyDown += OnKeyDown;
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        WindowSizing.AjustarAlAreaDeTrabajo(this);

        // En acciones destructivas el foco inicial va al botón seguro ("No").
        if (_esDestructiva && _cancelButton.IsVisible)
        {
            _cancelButton.Focus();
        }
        else
        {
            _acceptButton.Focus();
        }
    }

    private void OnAcceptClick(object? sender, RoutedEventArgs e) => Close(true);

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close(false);

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close(false);
        }
    }
}

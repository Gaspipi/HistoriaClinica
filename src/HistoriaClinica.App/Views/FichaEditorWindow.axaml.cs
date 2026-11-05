using Avalonia.Controls;
using HistoriaClinica.App.Layout;
using HistoriaClinica.App.ViewModels;

namespace HistoriaClinica.App.Views;

public partial class FichaEditorWindow : Window
{
    private readonly FichaEditorViewModel _viewModel;
    private readonly TextBox _motivoTextBox;

    public FichaEditorWindow(FichaEditorViewModel viewModel)
    {
        InitializeComponent();

        Responsive.Attach(this, compactWidth: 620, narrowWidth: 520, shortHeight: 520);

        _viewModel = viewModel;
        DataContext = viewModel;
        Icon = AppIcons.Load();

        _motivoTextBox = this.FindControl<TextBox>("MotivoTextBox")
            ?? throw new InvalidOperationException("No se encontró el control MotivoTextBox");

        _viewModel.CloseRequested += OnCloseRequested;
        Closing += OnClosing;
        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        try
        {
            WindowSizing.AjustarAlAreaDeTrabajo(this);
            await _viewModel.InitializeAsync();
            _motivoTextBox.Focus();
        }
        catch (Exception)
        {
            _viewModel.MensajeError = "No se pudieron cargar las plantillas. Intentá nuevamente.";
        }
    }

    private void FechaDatePicker_SelectedDateChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
    {
        if (e.NewDate is DateTimeOffset nuevaFecha)
        {
            var hora = _viewModel.FechaCreacion?.TimeOfDay ?? TimeSpan.Zero;
            _viewModel.FechaCreacion = nuevaFecha.Date + hora;
        }
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_viewModel.CierreConfirmado || !_viewModel.HayCambios)
        {
            return;
        }

        if (_viewModel.IsBusy)
        {
            e.Cancel = true;
            return;
        }

        e.Cancel = true;
        await _viewModel.SolicitarCancelacionAsync();
    }

    private void OnCloseRequested(object? sender, bool guardado) => Close(guardado);

    protected override void OnClosed(EventArgs e)
    {
        _viewModel.CloseRequested -= OnCloseRequested;
        base.OnClosed(e);
    }
}

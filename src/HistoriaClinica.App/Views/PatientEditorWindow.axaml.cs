using Avalonia.Controls;
using HistoriaClinica.App.Layout;
using HistoriaClinica.App.ViewModels;

namespace HistoriaClinica.App.Views;

public partial class PatientEditorWindow : Window
{
    private readonly PatientEditorViewModel _viewModel;
    private readonly TextBox _dniTextBox;
    private readonly TextBox _firstNameTextBox;

    public PatientEditorWindow(PatientEditorViewModel viewModel)
    {
        InitializeComponent();

        Responsive.Attach(this, compactWidth: 820, narrowWidth: 640, shortHeight: 560);

        _viewModel = viewModel;
        DataContext = viewModel;
        Icon = AppIcons.Load();

        _dniTextBox = this.FindControl<TextBox>("DniTextBox")!;
        _firstNameTextBox = this.FindControl<TextBox>("FirstNameTextBox")!;

        _viewModel.CloseRequested += OnCloseRequested;
        Closing += OnClosing;
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        WindowSizing.AjustarAlAreaDeTrabajo(this);

        if (_viewModel.DniEditable)
        {
            _dniTextBox.Focus();
        }
        else
        {
            _firstNameTextBox.Focus();
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

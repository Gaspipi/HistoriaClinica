using Avalonia.Controls;
using HistoriaClinica.App.Layout;
using HistoriaClinica.App.ViewModels;

namespace HistoriaClinica.App.Views;

public partial class PreferencesWindow : Window
{
    private readonly PreferencesViewModel _viewModel;

    public PreferencesWindow(PreferencesViewModel viewModel)
    {
        InitializeComponent();

        Responsive.Attach(this, compactWidth: 460, narrowWidth: 380, shortHeight: 560);

        _viewModel = viewModel;
        DataContext = viewModel;
        Icon = AppIcons.Load();

        _viewModel.CloseRequested += OnCloseRequested;
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e) =>
        WindowSizing.AjustarAlAreaDeTrabajo(this);

    private void OnCloseRequested(object? sender, EventArgs e) => Close();

    protected override void OnClosed(EventArgs e)
    {
        _viewModel.CloseRequested -= OnCloseRequested;
        base.OnClosed(e);
    }
}

using Avalonia.Controls;
using HistoriaClinica.App.Layout;
using HistoriaClinica.App.ViewModels;

namespace HistoriaClinica.App.Views;

public partial class DataMaintenanceWindow : Window
{
    private readonly DataMaintenanceViewModel _viewModel;

    public DataMaintenanceWindow(DataMaintenanceViewModel viewModel)
    {
        InitializeComponent();

        Responsive.Attach(this, compactWidth: 640, narrowWidth: 520, shortHeight: 480);

        _viewModel = viewModel;
        DataContext = viewModel;
        Icon = AppIcons.Load();
        _viewModel.CloseRequested += (_, _) => Close();
        Opened += (_, _) => WindowSizing.AjustarAlAreaDeTrabajo(this);
    }
}

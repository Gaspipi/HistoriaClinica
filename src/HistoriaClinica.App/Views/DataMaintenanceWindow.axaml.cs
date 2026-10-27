using Avalonia.Controls;
using HistoriaClinica.App.ViewModels;

namespace HistoriaClinica.App.Views;

public partial class DataMaintenanceWindow : Window
{
    private readonly DataMaintenanceViewModel _viewModel;

    public DataMaintenanceWindow(DataMaintenanceViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        _viewModel.CloseRequested += (_, _) => Close();
    }
}

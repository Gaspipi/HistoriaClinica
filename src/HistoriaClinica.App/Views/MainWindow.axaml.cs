using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using HistoriaClinica.App.Layout;
using HistoriaClinica.App.Services;
using HistoriaClinica.App.ViewModels;

namespace HistoriaClinica.App.Views;

public partial class MainWindow : Window
{
    private const double NarrowWidth = 760;
    private readonly MainWindowViewModel _viewModel;
    private readonly AutoCompleteBox _dniBox;
    private readonly ListBox _fichasList;
    private readonly DispatcherTimer _statusTimer;

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        Responsive.Attach(this, compactWidth: 1020, narrowWidth: NarrowWidth);

        _viewModel = viewModel;
        DataContext = viewModel;
        Icon = AppIcons.Load();

        _dniBox = this.FindControl<AutoCompleteBox>("DniBox")!;
        _fichasList = this.FindControl<ListBox>("FichasList")!;
        _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _statusTimer.Tick += OnStatusTimerTick;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _dniBox.KeyDown += OnDniKeyDown;
        _fichasList.KeyDown += OnFichasKeyDown;
        _fichasList.DoubleTapped += OnFichaDoubleTapped;
        KeyDown += OnWindowKeyDown;
        SizeChanged += OnSizeChanged;
        UiMetrics.Changed += OnUiMetricsChanged;
        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        WindowSizing.AjustarAlAreaDeTrabajo(this);
        ActualizarHistorial();
        await _viewModel.InitializeAsync();
        EnfocarDni(seleccionarTodo: true);
    }

    private void OnDniKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter
            && !_dniBox.IsDropDownOpen
            && _viewModel.BuscarCommand.CanExecute(null))
        {
            _viewModel.BuscarCommand.Execute(null);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            if (_dniBox.IsDropDownOpen)
            {
                _dniBox.IsDropDownOpen = false;
            }
            else
            {
                _viewModel.LimpiarBusquedaCommand.Execute(null);
                EnfocarDni(seleccionarTodo: true);
            }

            e.Handled = true;
        }
    }

    private void OnFichasKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete && _viewModel.BorrarFichaCommand.CanExecute(null))
        {
            _viewModel.BorrarFichaCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void OnFichaDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_viewModel.FichaSeleccionada is not null
            && _viewModel.EditarFichaCommand.CanExecute(null))
        {
            _viewModel.EditarFichaCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e) => ActualizarHistorial();

    private void OnUiMetricsChanged(object? sender, EventArgs e) => ActualizarHistorial();

    private void ActualizarHistorial()
    {
        var angosto = ClientSize.Width < NarrowWidth * UiMetrics.Scale;
        HistoryLayout.ColumnDefinitions = new ColumnDefinitions(angosto ? "240,0,*" : "320,6,*");
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            EnfocarDni(seleccionarTodo: true);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            _viewModel.LimpiarBusquedaCommand.Execute(null);
            EnfocarDni(seleccionarTodo: true);
            e.Handled = true;
        }
    }

    private void EnfocarDni(bool seleccionarTodo)
    {
        _dniBox.Focus();

        if (!seleccionarTodo)
        {
            return;
        }

        _dniBox.ApplyTemplate();
        _dniBox.GetVisualDescendants()
            .OfType<TextBox>()
            .FirstOrDefault()
            ?.SelectAll();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MainWindowViewModel.MensajeEstado))
        {
            return;
        }

        _statusTimer.Stop();

        if (_viewModel.HayMensajeEstado)
        {
            _statusTimer.Start();
        }
    }

    private void OnStatusTimerTick(object? sender, EventArgs e)
    {
        _statusTimer.Stop();
        _viewModel.LimpiarMensajeCommand.Execute(null);
    }

    protected override void OnClosed(EventArgs e)
    {
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _statusTimer.Stop();
        _statusTimer.Tick -= OnStatusTimerTick;
        SizeChanged -= OnSizeChanged;
        UiMetrics.Changed -= OnUiMetricsChanged;
        base.OnClosed(e);
    }
}

using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using HistoriaClinica.App.Layout;
using HistoriaClinica.App.Services;
using HistoriaClinica.App.ViewModels;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App.Views;

public partial class MainWindow : Window
{
    private const double NarrowWidth = 760;
    private readonly MainWindowViewModel _viewModel;
    private readonly ILogger<MainWindow> _logger;
    private readonly TextBox _buscador;
    private readonly ListBox _fichasList;
    private readonly DispatcherTimer _statusTimer;

    public MainWindow(MainWindowViewModel viewModel, ILogger<MainWindow> logger)
    {
        InitializeComponent();

        Responsive.Attach(this, compactWidth: 1020, narrowWidth: NarrowWidth);

        _viewModel = viewModel;
        _logger = logger;
        DataContext = viewModel;
        Icon = AppIcons.Load();

        _buscador = this.FindControl<TextBox>("BuscadorBox")
            ?? throw new InvalidOperationException("No se encontró el control BuscadorBox");
        _fichasList = this.FindControl<ListBox>("FichasList")
            ?? throw new InvalidOperationException("No se encontró el control FichasList");
        _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _statusTimer.Tick += OnStatusTimerTick;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _buscador.KeyDown += OnBuscadorKeyDown;
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
        try
        {
            WindowSizing.AjustarAlAreaDeTrabajo(this);
            ActualizarHistorial();
            await _viewModel.InitializeAsync();
            EnfocarBuscador(seleccionarTodo: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al abrir la ventana principal");
            _viewModel.MensajeError = "No se pudo abrir la ventana principal. Intentá nuevamente.";
        }
    }

    private void OnBuscadorKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter
            && _viewModel.BuscarCommand.CanExecute(null))
        {
            _viewModel.BuscarCommand.Execute(null);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            _viewModel.LimpiarBusquedaCommand.Execute(null);
            EnfocarBuscador(seleccionarTodo: true);
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
        var angosto = Responsive.EsNarrow(this);
        // En narrow el detalle va a la columna 1 (ver estilo Window.narrow Border#FichaDetail),
        // por eso se usan solo dos columnas: lista fija + detalle.
        HistoryLayout.ColumnDefinitions = new ColumnDefinitions(angosto ? "240,*" : "320,6,*");
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            EnfocarBuscador(seleccionarTodo: true);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            _viewModel.LimpiarBusquedaCommand.Execute(null);
            EnfocarBuscador(seleccionarTodo: true);
            e.Handled = true;
        }
    }

    private void EnfocarBuscador(bool seleccionarTodo)
    {
        _buscador.Focus();

        if (seleccionarTodo)
        {
            _buscador.SelectAll();
        }
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

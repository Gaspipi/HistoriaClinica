using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HistoriaClinica.App.Services;
using HistoriaClinica.App.ViewModels;
using HistoriaClinica.App.Views;
using HistoriaClinica.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.App;

public partial class App : Application
{
    private ServiceProvider? _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
                _services?.GetService<ILogger<App>>()?.LogError(ex, "Excepción no controlada");
        };
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            _services?.GetService<ILogger<App>>()?.LogError(e.Exception, "Task exception no observada");
            e.SetObserved();
        };
        Dispatcher.UnhandledException += (_, e) =>
        {
            _services?.GetService<ILogger<App>>()?.LogError(e.Exception, "Excepción no controlada en Dispatcher");
            e.Handled = true;
        };

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            services.AddHistoriaClinicaCore();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<UserSession>();
            services.AddSingleton<IAppNavigator, AppNavigator>();
            services.AddSingleton<IClipboardService, ClipboardService>();
            services.AddSingleton<IDataMaintenanceService, DataMaintenanceService>();
            services.AddSingleton<IUserPreferencesService, UserPreferencesService>();
            services.AddTransient<AuthViewModel>();
            services.AddTransient<AuthWindow>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<MainWindow>();
            services.AddTransient<PreferencesViewModel>();

            var provider = services.BuildServiceProvider();
            _services = provider;
            provider.GetRequiredService<IUserPreferencesService>().Inicializar();
            provider.GetRequiredService<IAppNavigator>().ShowAuth();

            desktop.ShutdownRequested += OnShutdownRequested;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        _services?.Dispose();
        _services = null;
    }
}

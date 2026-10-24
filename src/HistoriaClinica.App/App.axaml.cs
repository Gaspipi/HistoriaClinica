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
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
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
            provider.GetRequiredService<IUserPreferencesService>().Inicializar();
            provider.GetRequiredService<IAppNavigator>().ShowAuth();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

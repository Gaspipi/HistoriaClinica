using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using HistoriaClinica.App.Views;
using Microsoft.Extensions.DependencyInjection;

namespace HistoriaClinica.App.Services;

public interface IAppNavigator
{
    void ShowAuth();

    void ShowMain();
}

public sealed class AppNavigator : IAppNavigator
{
    private readonly IServiceProvider _services;

    public AppNavigator(IServiceProvider services)
    {
        _services = services;
    }

    public void ShowAuth() =>
        Mostrar(_services.GetRequiredService<AuthWindow>());

    public void ShowMain() =>
        Mostrar(_services.GetRequiredService<MainWindow>());

    private static void Mostrar(Window siguiente)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            siguiente.Show();
            return;
        }

        var anterior = desktop.MainWindow;
        desktop.MainWindow = siguiente;
        siguiente.Show();

        if (anterior is not null && !ReferenceEquals(anterior, siguiente))
        {
            anterior.Close();
        }
    }
}

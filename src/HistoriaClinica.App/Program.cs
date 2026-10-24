using System.Globalization;
using Avalonia;
using Avalonia.Fonts.Inter;
using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.App;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = FechaClinica.Culture;
        CultureInfo.DefaultThreadCurrentUICulture = FechaClinica.Culture;

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

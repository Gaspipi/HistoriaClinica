using Avalonia.Controls;
using Avalonia.Platform;

namespace HistoriaClinica.App.Views;

internal static class AppIcons
{
    public static WindowIcon Load() =>
        new(AssetLoader.Open(new Uri("avares://HistoriaClinica/Assets/Icons/historia-clinica.ico")));
}

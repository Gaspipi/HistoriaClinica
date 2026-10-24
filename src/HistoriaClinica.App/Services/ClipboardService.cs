using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

namespace HistoriaClinica.App.Services;

public interface IClipboardService
{
    Task CopiarAsync(string texto);
}

public sealed class ClipboardService : IClipboardService
{
    public async Task CopiarAsync(string texto)
    {
        var ventana = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow;

        var clipboard = ventana?.Clipboard;
        if (clipboard is null)
        {
            throw new InvalidOperationException("No se pudo acceder al portapapeles.");
        }

        await clipboard.SetTextAsync(texto);
    }
}

using Avalonia.Controls;
using Avalonia.VisualTree;
using HistoriaClinica.App.Services;

namespace HistoriaClinica.App.Layout;

public static class Responsive
{
    private static readonly Dictionary<Window, WindowBreakpoints> Ventanas = new();

    static Responsive()
    {
        UiMetrics.Changed += (_, _) => RefreshAll();
    }

    public static void Attach(
        Window window,
        double compactWidth,
        double narrowWidth,
        double shortHeight = 620)
    {
        if (Ventanas.ContainsKey(window))
        {
            return;
        }

        Ventanas[window] = new WindowBreakpoints(compactWidth, narrowWidth, shortHeight);
        window.Opened += OnOpened;
        window.SizeChanged += OnSizeChanged;
        window.Closed += OnClosed;

        Refresh(window);
    }

    public static void RefreshAll()
    {
        foreach (var ventana in Ventanas.Keys.ToArray())
        {
            Refresh(ventana);
            ventana.InvalidateMeasure();

            foreach (var grid in ventana.GetVisualDescendants().OfType<ResponsiveGrid>())
            {
                grid.InvalidateMeasure();
            }
        }
    }

    private static void OnOpened(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            Refresh(window);
        }
    }

    private static void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (sender is Window window)
        {
            Refresh(window);
        }
    }

    private static void OnClosed(object? sender, EventArgs e)
    {
        if (sender is not Window window)
        {
            return;
        }

        window.Opened -= OnOpened;
        window.SizeChanged -= OnSizeChanged;
        window.Closed -= OnClosed;
        Ventanas.Remove(window);
    }

    private static void Refresh(Window window)
    {
        if (!Ventanas.TryGetValue(window, out var breakpoints))
        {
            return;
        }

        var escala = UiMetrics.Scale;
        var ancho = window.ClientSize.Width;
        var alto = window.ClientSize.Height;

        AplicarClase(
            window,
            "compact",
            ancho < breakpoints.CompactWidth * escala
            || alto < breakpoints.ShortHeight * escala);
        AplicarClase(window, "narrow", ancho < breakpoints.NarrowWidth * escala);
        AplicarClase(window, "short", alto < breakpoints.ShortHeight * escala);
    }

    private static void AplicarClase(Window window, string clase, bool activa)
    {
        if (activa)
        {
            if (!window.Classes.Contains(clase))
            {
                window.Classes.Add(clase);
            }

            return;
        }

        window.Classes.Remove(clase);
    }

    private sealed record WindowBreakpoints(
        double CompactWidth,
        double NarrowWidth,
        double ShortHeight);
}

using Avalonia.Controls;

namespace HistoriaClinica.App.Layout;

public static class WindowSizing
{
    public static void AjustarAlAreaDeTrabajo(Window window, double margen = 24)
    {
        var pantalla = window.Screens.ScreenFromWindow(window) ?? window.Screens.Primary;
        if (pantalla is null)
        {
            return;
        }

        var escala = pantalla.Scaling > 0 ? pantalla.Scaling : 1;
        var area = pantalla.WorkingArea.ToRect(escala);
        var anchoDisponible = Math.Max(320, area.Width - margen);
        var altoDisponible = Math.Max(320, area.Height - margen);

        if (window.MinWidth > anchoDisponible)
        {
            window.MinWidth = anchoDisponible;
        }

        if (window.MinHeight > altoDisponible)
        {
            window.MinHeight = altoDisponible;
        }

        if (!double.IsNaN(window.Width) && window.Width > anchoDisponible)
        {
            window.Width = anchoDisponible;
        }

        if (window.SizeToContent.HasFlag(SizeToContent.Width))
        {
            window.MaxWidth = anchoDisponible;
        }

        if (window.SizeToContent.HasFlag(SizeToContent.Height))
        {
            window.MaxHeight = altoDisponible;
        }
        else if (!double.IsNaN(window.Height) && window.Height > altoDisponible)
        {
            window.Height = altoDisponible;
        }
    }
}

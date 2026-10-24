using Avalonia;
using Avalonia.Controls;
using HistoriaClinica.App.Services;

namespace HistoriaClinica.App.Layout;

public sealed class ResponsiveGrid : Panel
{
    public static readonly StyledProperty<int> MaxColumnsProperty =
        AvaloniaProperty.Register<ResponsiveGrid, int>(nameof(MaxColumns), 1);

    public static readonly StyledProperty<double> MinColumnWidthProperty =
        AvaloniaProperty.Register<ResponsiveGrid, double>(nameof(MinColumnWidth), 240);

    public static readonly StyledProperty<double> ColumnSpacingProperty =
        AvaloniaProperty.Register<ResponsiveGrid, double>(nameof(ColumnSpacing));

    public static readonly StyledProperty<double> RowSpacingProperty =
        AvaloniaProperty.Register<ResponsiveGrid, double>(nameof(RowSpacing));

    static ResponsiveGrid()
    {
        AffectsMeasure<ResponsiveGrid>(
            MaxColumnsProperty,
            MinColumnWidthProperty,
            ColumnSpacingProperty,
            RowSpacingProperty);
        AffectsArrange<ResponsiveGrid>(
            MaxColumnsProperty,
            MinColumnWidthProperty,
            ColumnSpacingProperty,
            RowSpacingProperty);
    }

    public int MaxColumns
    {
        get => GetValue(MaxColumnsProperty);
        set => SetValue(MaxColumnsProperty, value);
    }

    public double MinColumnWidth
    {
        get => GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
    }

    public double ColumnSpacing
    {
        get => GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    public double RowSpacing
    {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var children = HijosVisibles();
        if (children.Count == 0)
        {
            return new Size(
                double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width,
                0);
        }

        var columnas = CalcularColumnas(availableSize.Width);
        var anchoHijo = CalcularAnchoHijo(availableSize.Width, columnas);
        var filas = (children.Count + columnas - 1) / columnas;
        var alturas = new double[filas];

        for (var i = 0; i < children.Count; i++)
        {
            var hijo = children[i];
            hijo.Measure(new Size(anchoHijo, double.PositiveInfinity));
            alturas[i / columnas] = Math.Max(alturas[i / columnas], hijo.DesiredSize.Height);
        }

        var alto = alturas.Sum() + RowSpacing * Math.Max(0, filas - 1);
        var ancho = double.IsInfinity(availableSize.Width)
            ? anchoHijo * columnas + ColumnSpacing * Math.Max(0, columnas - 1)
            : availableSize.Width;

        return new Size(ancho, alto);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = HijosVisibles();
        if (children.Count == 0)
        {
            return finalSize;
        }

        var columnas = CalcularColumnas(finalSize.Width);
        var anchoHijo = CalcularAnchoHijo(finalSize.Width, columnas);
        var filas = (children.Count + columnas - 1) / columnas;
        var alturas = new double[filas];

        for (var i = 0; i < children.Count; i++)
        {
            alturas[i / columnas] = Math.Max(alturas[i / columnas], children[i].DesiredSize.Height);
        }

        var y = 0d;
        for (var fila = 0; fila < filas; fila++)
        {
            for (var columna = 0; columna < columnas; columna++)
            {
                var indice = fila * columnas + columna;
                if (indice >= children.Count)
                {
                    break;
                }

                var x = columna * (anchoHijo + ColumnSpacing);
                children[indice].Arrange(new Rect(x, y, anchoHijo, alturas[fila]));
            }

            y += alturas[fila] + RowSpacing;
        }

        return finalSize;
    }

    private int CalcularColumnas(double anchoDisponible)
    {
        var maximo = Math.Max(1, MaxColumns);
        if (maximo == 1)
        {
            return 1;
        }

        var minimo = Math.Max(1, MinColumnWidth * UiMetrics.Scale);
        if (double.IsInfinity(anchoDisponible) || anchoDisponible <= 0)
        {
            return maximo;
        }

        var columnas = (int)Math.Floor(
            (anchoDisponible + ColumnSpacing) / (minimo + ColumnSpacing));

        return Math.Clamp(columnas, 1, maximo);
    }

    private double CalcularAnchoHijo(double anchoDisponible, int columnas)
    {
        if (double.IsInfinity(anchoDisponible) || anchoDisponible <= 0)
        {
            return Math.Max(1, MinColumnWidth * UiMetrics.Scale);
        }

        var espacioColumnas = ColumnSpacing * Math.Max(0, columnas - 1);
        return Math.Max(1, (anchoDisponible - espacioColumnas) / columnas);
    }

    private List<Control> HijosVisibles() =>
        Children.Where(hijo => hijo.IsVisible).ToList();
}

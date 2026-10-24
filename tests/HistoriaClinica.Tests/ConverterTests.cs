using System.Globalization;
using HistoriaClinica.App.Converters;

namespace HistoriaClinica.Tests;

public sealed class ConverterTests
{
    private static readonly CultureInfo Cultura = CultureInfo.InvariantCulture;

    [Fact]
    public void EmptyText_ReemplazaValoresVacios()
    {
        var converter = new EmptyTextConverter();

        Assert.Equal("Sin datos", converter.Convert(null, typeof(string), "Sin datos", Cultura));
        Assert.Equal("Sin datos", converter.Convert("   ", typeof(string), "Sin datos", Cultura));
        Assert.Equal("Sin datos", converter.Convert(default(DateTime), typeof(string), "Sin datos", Cultura));
        Assert.Equal("Ana", converter.Convert("Ana", typeof(string), "Sin datos", Cultura));
    }

    [Fact]
    public void Fecha_FormateaSegunElParametro()
    {
        var converter = new FechaConverter();
        var fecha = new DateTime(2026, 10, 22, 14, 35, 12);

        Assert.Equal("22/10/2026", converter.Convert(fecha, typeof(string), "date", Cultura));
        Assert.Equal("14:35", converter.Convert(fecha, typeof(string), "time", Cultura));
        Assert.Equal("22/10/2026 14:35:12", converter.Convert(fecha, typeof(string), null, Cultura));
        Assert.Equal(string.Empty, converter.Convert(default(DateTime), typeof(string), null, Cultura));
    }

    [Fact]
    public void Fecha_NoSoportaConvertBack()
    {
        var converter = new FechaConverter();

        Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack("22/10/2026", typeof(DateTime), null, Cultura));
    }

    [Fact]
    public void DateTimeOffset_ConvierteIdaYVuelta()
    {
        var converter = new DateTimeOffsetConverter();
        var fecha = new DateTime(1990, 5, 4);

        var offset = Assert.IsType<DateTimeOffset>(converter.Convert(fecha, typeof(DateTimeOffset?), null, Cultura));
        var vuelta = Assert.IsType<DateTime>(converter.ConvertBack(offset, typeof(DateTime?), null, Cultura));

        Assert.Equal(fecha, vuelta);
    }

    [Fact]
    public void DateTimeOffset_DevuelveNullSinFecha()
    {
        var converter = new DateTimeOffsetConverter();

        Assert.Null(converter.Convert(null, typeof(DateTimeOffset?), null, Cultura));
        Assert.Null(converter.ConvertBack(null, typeof(DateTime?), null, Cultura));
    }
}

using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.Tests;

public class FechaClinicaTests
{
    [Fact]
    public void FormatDate_UsaFormatoArgentino()
    {
        Assert.Equal("31/12/1990", FechaClinica.FormatDate(new DateTime(1990, 12, 31)));
    }

    [Fact]
    public void FormatDateTime_UsaFormatoArgentinoConHora()
    {
        Assert.Equal("31/12/1990 08:05:09", FechaClinica.FormatDateTime(new DateTime(1990, 12, 31, 8, 5, 9)));
    }

    [Fact]
    public void TryParseDate_AceptaDiaMesAnio()
    {
        var ok = FechaClinica.TryParseDate("31/12/1990", out var fecha);

        Assert.True(ok);
        Assert.Equal(new DateTime(1990, 12, 31), fecha);
    }

    [Fact]
    public void TryParseDate_RechazaFormatoMesDiaAnio()
    {
        Assert.False(FechaClinica.TryParseDate("12/31/1990", out _));
    }

    [Fact]
    public void HasValue_ConsideraMinValueComoVacio()
    {
        Assert.False(FechaClinica.HasValue(DateTime.MinValue));
        Assert.Equal(string.Empty, FechaClinica.FormatDate(DateTime.MinValue));
    }

    [Fact]
    public void TryParseDateTime_AceptaFechaConHora()
    {
        var ok = FechaClinica.TryParseDateTime("31/12/1990 08:05:09", out var fecha);

        Assert.True(ok);
        Assert.Equal(new DateTime(1990, 12, 31, 8, 5, 9), fecha);
    }
}

using HistoriaClinica.Core.Data;

namespace HistoriaClinica.Tests;

public class DatabasePathProviderTests
{
    [Fact]
    public void RutaExplicita_SeUsaTalCual()
    {
        var ruta = Path.Combine(Path.GetTempPath(), "historia-custom.db");

        var provider = new DatabasePathProvider(ruta);

        Assert.Equal(Path.GetFullPath(ruta), provider.DatabasePath);
    }

    [Fact]
    public void RutaPorDefecto_TerminaConElNombreDeLaBase()
    {
        var provider = new DatabasePathProvider();

        Assert.Equal("Historia.db", Path.GetFileName(provider.DatabasePath));
    }
}

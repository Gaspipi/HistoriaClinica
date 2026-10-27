namespace HistoriaClinica.Core.Data;

public sealed class DatabasePathProvider : IDatabasePathProvider
{
    public const string OverrideEnvironmentVariable = "HISTORIA_DB_PATH";
    private const string DatabaseFileName = "Historia.db";

    public DatabasePathProvider() : this(null)
    {
    }

    public DatabasePathProvider(string? overridePath)
    {
        DatabasePath = Resolve(overridePath);
    }

    public string DatabasePath { get; }

    private static string Resolve(string? overridePath)
    {
        var environmentPath = Environment.GetEnvironmentVariable(OverrideEnvironmentVariable);
        var candidate = FirstNonEmpty(overridePath, environmentPath);
        if (candidate is not null)
        {
            var fullPath = Path.GetFullPath(candidate);
            if (!string.Equals(Path.GetExtension(fullPath), ".db", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("La ruta de base de datos debe tener extensión .db.");
            }
            return fullPath;
        }

        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (string.IsNullOrWhiteSpace(documents))
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (string.IsNullOrWhiteSpace(home))
            {
                home = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }

            var spanishDocuments = Path.Combine(home, "Documentos");
            documents = Directory.Exists(spanishDocuments)
                ? spanishDocuments
                : Path.Combine(home, "Documents");
        }

        if (string.IsNullOrWhiteSpace(documents))
        {
            documents = AppContext.BaseDirectory;
        }

        return Path.Combine(documents, DatabaseFileName);
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }
}

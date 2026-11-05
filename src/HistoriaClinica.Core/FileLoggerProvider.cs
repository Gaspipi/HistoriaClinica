using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Logging;

namespace HistoriaClinica.Core;

/// <summary>
/// Alias de <see cref="Logging.FileLoggerProvider"/> en el espacio de nombres raíz.
/// </summary>
public class FileLoggerProvider : HistoriaClinica.Core.Logging.FileLoggerProvider
{
    public FileLoggerProvider(string logDirectory, CancellationToken cancellationToken = default)
        : base(logDirectory, cancellationToken)
    {
    }

    public FileLoggerProvider(IDatabasePathProvider pathProvider, CancellationToken cancellationToken = default)
        : base(pathProvider, cancellationToken)
    {
    }
}

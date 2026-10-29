using HistoriaClinica.Core.Data;
using HistoriaClinica.Core.Logging;

namespace HistoriaClinica.App.Logging;

public class FileLoggerProvider : Core.Logging.FileLoggerProvider
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

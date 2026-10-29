using HistoriaClinica.Core.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging;

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, HistoriaClinica.Core.Logging.FileLoggerProvider>(
                sp => new HistoriaClinica.Core.Logging.FileLoggerProvider(sp.GetRequiredService<IDatabasePathProvider>())));
        return builder;
    }

    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, string logDirectory)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, HistoriaClinica.Core.Logging.FileLoggerProvider>(
                _ => new HistoriaClinica.Core.Logging.FileLoggerProvider(logDirectory)));
        return builder;
    }

    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, IDatabasePathProvider pathProvider)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, HistoriaClinica.Core.Logging.FileLoggerProvider>(
                _ => new HistoriaClinica.Core.Logging.FileLoggerProvider(pathProvider)));
        return builder;
    }

    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, string logDirectory, CancellationToken cancellationToken)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, HistoriaClinica.Core.Logging.FileLoggerProvider>(
                _ => new HistoriaClinica.Core.Logging.FileLoggerProvider(logDirectory, cancellationToken)));
        return builder;
    }
}

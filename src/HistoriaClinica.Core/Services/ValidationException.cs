namespace HistoriaClinica.Core.Services;

public sealed class ValidationException : Exception
{
    public ValidationException(IReadOnlyList<string> errors)
        : base(string.Join(Environment.NewLine, errors))
    {
        Errors = errors;
    }

    public IReadOnlyList<string> Errors { get; }
}

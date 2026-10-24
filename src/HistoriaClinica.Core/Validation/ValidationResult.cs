namespace HistoriaClinica.Core.Validation;

public sealed class ValidationResult
{
    public static readonly ValidationResult Success = new(Array.Empty<string>());

    private ValidationResult(IReadOnlyList<string> errors)
    {
        Errors = errors;
    }

    public IReadOnlyList<string> Errors { get; }

    public bool IsValid => Errors.Count == 0;

    public string ErrorMessage => string.Join(Environment.NewLine, Errors);

    public static ValidationResult Fail(params string[] errors) => new(errors);

    public static ValidationResult Fail(IEnumerable<string> errors) => new(errors.ToArray());
}

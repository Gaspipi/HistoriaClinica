using HistoriaClinica.Core.Models;

namespace HistoriaClinica.Core.Validation;

public static class HistoriaValidators
{
    public static ValidationResult ValidatePaciente(Paciente paciente)
    {
        var errors = new List<string>();

        if (!DniValidator.IsValid(paciente.Dni))
        {
            errors.Add(DniValidator.ErrorMessage);
        }

        if (paciente.DOB.HasValue && paciente.DOB.Value.ToDateTime(TimeOnly.MinValue) > DateTime.Today)
        {
            errors.Add("La fecha de nacimiento no puede ser posterior a hoy.");
        }

        return errors.Count == 0 ? ValidationResult.Success : ValidationResult.Fail(errors);
    }

    public static ValidationResult ValidateFicha(FichaDiaria ficha)
    {
        var errors = new List<string>();

        if (!DniValidator.IsValid(ficha.Dni))
        {
            errors.Add(DniValidator.ErrorMessage);
        }

        if (string.IsNullOrWhiteSpace(ficha.Motivo))
        {
            errors.Add("El campo Motivo es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(ficha.Enfermedad))
        {
            errors.Add("El campo Enfermedad es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(ficha.Indicaciones))
        {
            errors.Add("El campo Indicaciones es obligatorio.");
        }

        if (!FechaClinica.HasValue(ficha.Fecha))
        {
            errors.Add("La fecha de la ficha no es válida.");
        }

        return errors.Count == 0 ? ValidationResult.Success : ValidationResult.Fail(errors);
    }
}

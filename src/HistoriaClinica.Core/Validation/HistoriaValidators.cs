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

    public static ValidationResult ValidatePlantilla(Plantilla plantilla)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(plantilla.Nombre))
        {
            errors.Add("El campo Nombre es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(plantilla.Motivo))
        {
            errors.Add("El campo Motivo es obligatorio.");
        }

        if (!string.IsNullOrWhiteSpace(plantilla.CreadorDni) && !DniValidator.IsValid(plantilla.CreadorDni))
        {
            errors.Add(DniValidator.ErrorMessage);
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

        if (!string.IsNullOrWhiteSpace(ficha.PresionArterial))
        {
            var partes = ficha.PresionArterial.Split('/');
            if (partes.Length != 2
                || !int.TryParse(partes[0], out var sistolica)
                || !int.TryParse(partes[1], out var diastolica)
                || sistolica < 40 || sistolica > 300
                || diastolica < 20 || diastolica > 200)
            {
                errors.Add("El formato de la presión arterial no es válido (ej: 120/80).");
            }
        }

        return errors.Count == 0 ? ValidationResult.Success : ValidationResult.Fail(errors);
    }
}

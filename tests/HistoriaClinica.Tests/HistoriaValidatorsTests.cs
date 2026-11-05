using HistoriaClinica.Core.Models;
using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.Tests;

public class HistoriaValidatorsTests
{
    [Fact]
    public void ValidatePaciente_ConDniInvalido_DevuelveError()
    {
        var paciente = new Paciente { Dni = "123" };

        var resultado = HistoriaValidators.ValidatePaciente(paciente);

        Assert.False(resultado.IsValid);
        Assert.Contains(DniValidator.ErrorMessage, resultado.Errors);
    }

    [Fact]
    public void ValidatePaciente_ConFechaFutura_DevuelveError()
    {
        var paciente = new Paciente
        {
            Dni = "12345678",
            FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
        };

        var resultado = HistoriaValidators.ValidatePaciente(paciente);

        Assert.False(resultado.IsValid);
    }

    [Fact]
    public void ValidatePaciente_SinFechaDeNacimiento_EsValido()
    {
        var paciente = new Paciente
        {
            Dni = "12345678",
            FechaNacimiento = null,
        };

        Assert.True(HistoriaValidators.ValidatePaciente(paciente).IsValid);
    }

    [Fact]
    public void ValidateFicha_SinCamposObligatorios_DevuelveTodosLosErrores()
    {
        var ficha = new FichaDiaria
        {
            Dni = "12345678",
            Fecha = DateTime.Now,
            Motivo = " ",
            Enfermedad = "",
            Indicaciones = null!,
        };

        var resultado = HistoriaValidators.ValidateFicha(ficha);

        Assert.False(resultado.IsValid);
        Assert.Equal(3, resultado.Errors.Count);
    }

    [Fact]
    public void ValidatePlantilla_SinNombreNiMotivo_DevuelveDosErrores()
    {
        var plantilla = new Plantilla { Nombre = "  ", Motivo = "" };

        var resultado = HistoriaValidators.ValidatePlantilla(plantilla);

        Assert.False(resultado.IsValid);
        Assert.Equal(2, resultado.Errors.Count);
        Assert.Contains("El campo Nombre es obligatorio.", resultado.Errors);
        Assert.Contains("El campo Motivo es obligatorio.", resultado.Errors);
    }

    [Fact]
    public void ValidatePlantilla_Valida_DevuelveSuccess()
    {
        var plantilla = new Plantilla
        {
            Nombre = "Control",
            Motivo = "Control de rutina",
            Enfermedad = "Sin particularidades",
            Indicaciones = "Reposo",
        };

        var resultado = HistoriaValidators.ValidatePlantilla(plantilla);

        Assert.True(resultado.IsValid);
        Assert.Empty(resultado.Errors);
    }

    [Fact]
    public void ValidatePlantilla_CreadorDniInvalido_DevuelveError()
    {
        var plantilla = new Plantilla
        {
            Nombre = "Control",
            Motivo = "Control de rutina",
            CreadorDni = "123",
        };

        var resultado = HistoriaValidators.ValidatePlantilla(plantilla);

        Assert.False(resultado.IsValid);
        Assert.Contains(DniValidator.ErrorMessage, resultado.Errors);
    }

    [Fact]
    public void ValidatePlantilla_CreadorDniVacio_Permitido()
    {
        var plantilla = new Plantilla
        {
            Nombre = "Control",
            Motivo = "Control de rutina",
            CreadorDni = "   ",
        };

        Assert.True(HistoriaValidators.ValidatePlantilla(plantilla).IsValid);
        Assert.True(HistoriaValidators.ValidatePlantilla(
            new Plantilla { Nombre = "Control", Motivo = "Motivo", CreadorDni = null }).IsValid);
    }

    [Theory]
    [InlineData("120/80")]
    [InlineData("40/20")]
    [InlineData("300/200")]
    [InlineData("")]
    [InlineData(null)]
    public void ValidateFicha_PresionArterial_Valida(string? presion)
    {
        var ficha = CrearFichaValida();
        ficha.PresionArterial = presion;

        var resultado = HistoriaValidators.ValidateFicha(ficha);

        Assert.True(resultado.IsValid);
        Assert.Empty(resultado.Errors);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("120")]
    [InlineData("120/80/90")]
    [InlineData("39/80")]
    [InlineData("301/80")]
    [InlineData("120/19")]
    [InlineData("120/201")]
    public void ValidateFicha_PresionArterial_Invalida(string presion)
    {
        var ficha = CrearFichaValida();
        ficha.PresionArterial = presion;

        var resultado = HistoriaValidators.ValidateFicha(ficha);

        Assert.False(resultado.IsValid);
        Assert.Contains(
            "El formato de la presión arterial no es válido (ej: 120/80).",
            resultado.Errors);
    }

    private static FichaDiaria CrearFichaValida() => new()
    {
        Dni = "12345678",
        Fecha = DateTime.Today,
        Motivo = "Control",
        Enfermedad = "Sin particularidades",
        Indicaciones = "Reposo",
    };
}

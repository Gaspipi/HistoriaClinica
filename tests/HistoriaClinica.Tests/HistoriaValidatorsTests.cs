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
            DOB = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
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
            DOB = null,
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
}

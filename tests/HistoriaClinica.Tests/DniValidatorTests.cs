using HistoriaClinica.Core.Validation;

namespace HistoriaClinica.Tests;

public class DniValidatorTests
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("00000000")]
    public void IsValid_ConOchoDigitos_DevuelveTrue(string dni)
    {
        Assert.True(DniValidator.IsValid(dni));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("1234567a")]
    [InlineData("12 45678")]
    [InlineData(null)]
    public void IsValid_ConValorInvalido_DevuelveFalse(string? dni)
    {
        Assert.False(DniValidator.IsValid(dni));
        Assert.Equal(DniValidator.ErrorMessage, DniValidator.GetError(dni));
    }
}

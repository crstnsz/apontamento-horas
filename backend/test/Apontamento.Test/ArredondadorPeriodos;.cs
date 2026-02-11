using System;
using Shouldly;
using Xunit;

namespace Apontamento.Domain;

public class ArredondadorPeriodoTest
{
    [Theory]
    [InlineData(8, 0, 8, 0)]
    [InlineData(8, 1, 8, 0)]
    [InlineData(8, 2, 8, 0)]
    public void PrefencialmentePraBaixo_MinutosEntre0e2_DeveTerminarEmZero(int hora, int minuto, int horaEsperada, int minutoEsperado)
    {
        var arredondador = new ArredondadorPeriodo(new TimeOnly(hora, minuto));
        var resultado = arredondador.PrefencialmentePraBaixo();
        resultado.Hour.ShouldBe(horaEsperada);
        resultado.Minute.ShouldBe(minutoEsperado);
    }

    [Theory]
    [InlineData(8, 3, 8, 5)]
    [InlineData(8, 4, 8, 5)]
    [InlineData(8, 6, 8, 5)]
    [InlineData(8, 7, 8, 5)]
    public void PrefencialmentePraBaixo_MinutosEntre3e7_DeveTerminarEmCinco(int hora, int minuto, int horaEsperada, int minutoEsperado)
    {
        var arredondador = new ArredondadorPeriodo(new TimeOnly(hora, minuto));
        var resultado = arredondador.PrefencialmentePraBaixo();
        resultado.Hour.ShouldBe(horaEsperada);
        resultado.Minute.ShouldBe(minutoEsperado);
    }

    [Theory]
    [InlineData(8, 8, 8, 10)]
    [InlineData(8, 9, 8, 10)]
    public void PrefencialmentePraBaixo_MinutosMaioresQue7_DeveIrParaProximaDezena(int hora, int minuto, int horaEsperada, int minutoEsperado)
    {
        var arredondador = new ArredondadorPeriodo(new TimeOnly(hora, minuto));
        var resultado = arredondador.PrefencialmentePraBaixo();
        resultado.Hour.ShouldBe(horaEsperada);
        resultado.Minute.ShouldBe(minutoEsperado);
    }

    [Theory]
    [InlineData(8, 0, 8, 0)]
    [InlineData(8, 5, 8, 5)]
    [InlineData(8, 10, 8, 10)]
    [InlineData(8, 15, 8, 15)]
    [InlineData(8, 20, 8, 20)]
    [InlineData(8, 25, 8, 25)]
    [InlineData(8, 30, 8, 30)]
    [InlineData(8, 35, 8, 35)]
    [InlineData(8, 40, 8, 40)]
    [InlineData(8, 45, 8, 45)]
    [InlineData(8, 50, 8, 50)]
    [InlineData(8, 55, 8, 55)]
    public void PrefencialmentePraBaixo_MinutosExatos_DeveManterValor(int hora, int minuto, int horaEsperada, int minutoEsperado)
    {
        var arredondador = new ArredondadorPeriodo(new TimeOnly(hora, minuto));
        var resultado = arredondador.PrefencialmentePraBaixo();
        resultado.Hour.ShouldBe(horaEsperada);
        resultado.Minute.ShouldBe(minutoEsperado);
    }
}
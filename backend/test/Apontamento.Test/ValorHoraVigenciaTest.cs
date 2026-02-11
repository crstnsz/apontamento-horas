using System;
using Shouldly;
using Xunit;

namespace Apontamento.Domain;

public class ValorHoraVigenciaTest
{
    [Fact]
    public void Deve_Criar_Vigencia_Valida_Com_DataFim()
    {
        // Arrange
        var inicio = new DateTime(2026, 1, 1);
        var fim = new DateTime(2026, 12, 31, 23, 59, 59);

        // Act
        var vigencia = ValorHoraVigencia.Criar(100m, inicio, fim);

        // Assert
        vigencia.ValorHora.ShouldBe(100m);
        vigencia.Inicio.ShouldBe(inicio);
        vigencia.Fim.ShouldBe(fim);
    }

    [Fact]
    public void Deve_Criar_Vigencia_Com_DataFim_Aberta()
    {
        // Arrange
        var inicio = new DateTime(2026, 1, 1);

        // Act
        var vigencia = ValorHoraVigencia.Criar(100m, inicio, null);

        // Assert
        vigencia.Fim.ShouldBe(new DateTime(9999, 12, 31, 23, 59, 59));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(10000.01)]
    public void Nao_Deve_Aceitar_ValorHora_Invalido(decimal valorInvalido)
    {
        // Arrange
        var inicio = new DateTime(2026, 1, 1);

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => ValorHoraVigencia.Criar(valorInvalido, inicio, null));
        ex.Message.ShouldContain("Custo por hora");
    }

    [Fact]
    public void Nao_Deve_Aceitar_Inicio_Maior_Que_Fim()
    {
        // Arrange
        var inicio = new DateTime(2026, 2, 1);
        var fim = new DateTime(2026, 1, 1);

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => ValorHoraVigencia.Criar(100m, inicio, fim));
        ex.Message.ShouldContain("Data de início");
    }

    [Fact]
    public void Deve_Conter_Data_Dentro_Do_Intervalo()
    {
        // Arrange
        var inicio = new DateTime(2026, 1, 1);
        var fim = new DateTime(2026, 12, 31, 23, 59, 59);
        var vigencia = ValorHoraVigencia.Criar(100m, inicio, fim);

        // Act
        var resultado = vigencia.Contem(new DateTime(2026, 6, 1));

        // Assert
        resultado.ShouldBeTrue();
    }

    [Fact]
    public void Nao_Deve_Conter_Data_Fora_Do_Intervalo()
    {
        // Arrange
        var inicio = new DateTime(2026, 1, 1);
        var fim = new DateTime(2026, 12, 31, 23, 59, 59);
        var vigencia = ValorHoraVigencia.Criar(100m, inicio, fim);

        // Act
        var resultado = vigencia.Contem(new DateTime(2025, 12, 31));

        // Assert
        resultado.ShouldBeFalse();
    }
}

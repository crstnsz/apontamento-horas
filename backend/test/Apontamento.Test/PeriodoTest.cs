using System;
using Shouldly;
using Xunit;

namespace Apontamento.Domain;

public class PeriodoTest
{
    [Fact]
    public void Deve_Criar_Periodo_Valido()
    {
        // Arrange
        var projeto = new Projeto("Projeto X", 100m);

        // Act
        var periodo = new Periodo("Desenvolvimento", projeto, new TimeOnly(8, 0), new TimeOnly(12, 0));

        // Assert
        periodo.DescricaoServico.ShouldBe("Desenvolvimento");
        periodo.Projeto.ShouldBe(projeto);
        periodo.Inicio.ShouldBe(new TimeOnly(8, 0));
        periodo.Fim.ShouldBe(new TimeOnly(12, 0));
        periodo.TotalHoras.ShouldBe(TimeSpan.FromHours(4));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Nao_Deve_Aceitar_Descricao_Servico_Invalida(string descricaoInvalida)
    {
        // Arrange
        var projeto = new Projeto("Projeto X", 100m);

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() =>
            new Periodo(descricaoInvalida, projeto, new TimeOnly(8, 0), new TimeOnly(9, 0)));

        ex.Message.ShouldContain("Descrição do serviço");
    }

    [Fact]
    public void Nao_Deve_Aceitar_Descricao_Com_Mais_De_100_Caracteres()
    {
        // Arrange
        var projeto = new Projeto("Projeto X", 100m);
        var descricao = new string('a', 101);

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() =>
            new Periodo(descricao, projeto, new TimeOnly(8, 0), new TimeOnly(9, 0)));

        ex.Message.ShouldContain("Descrição do serviço");
    }

    [Fact]
    public void Nao_Deve_Aceitar_Projeto_Nulo()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new Periodo("Serviço", null!, new TimeOnly(8, 0), new TimeOnly(9, 0)));
    }

    [Fact]
    public void Nao_Deve_Aceitar_Fim_Menor_Ou_Igual_Inicio()
    {
        // Arrange
        var projeto = new Projeto("Projeto X", 100m);

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() =>
            new Periodo("Serviço", projeto, new TimeOnly(10, 0), new TimeOnly(10, 0)));

        ex.Message.ShouldContain("Hora fim");
    }

    [Fact]
    public void Deve_Calcular_Valor_Total_Por_Data()
    {
        // Arrange
        var projeto = new Projeto("Projeto X", 100m);
        projeto.DefinirValorHora(new DateTime(2026, 1, 1), null, 120m);

        var periodo = new Periodo("Serviço", projeto, new TimeOnly(8, 0), new TimeOnly(10, 0));

        // Act
        var valor = periodo.CalcularValorTotal(new DateOnly(2026, 2, 1));

        // Assert
        valor.ShouldBe(240m);
    }
}

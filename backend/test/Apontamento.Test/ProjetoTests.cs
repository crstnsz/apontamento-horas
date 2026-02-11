using System;
using Xunit;
using Shouldly;

namespace Apontamento.Domain;

public class ProjetoTest
{
    [Fact]
    public void Deve_Criar_Projeto_Valido()
    {
        // Arrange
        var nome = "Projeto X";
        var custoHora = 150.50m;

        // Act
        var projeto = new Projeto(nome, custoHora);

        // Assert
        projeto.Nome.ShouldBe(nome);
        projeto.ObterValorHora(DateTime.Now).ShouldBe(custoHora);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Nao_Deve_Aceitar_Nome_Nulo_Ou_Vazio(string? nomeInvalido)
    {
        // Arrange
        var custoHora = 100m;

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => new Projeto(nomeInvalido, custoHora));
        ex.Message.ShouldContain("Nome do projeto");
    }

    [Fact]
    public void Nao_Deve_Aceitar_Nome_Com_Mais_De_100_Caracteres()
    {
        // Arrange
        var nomeInvalido = new string('a', 101);
        var custoHora = 100m;

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => new Projeto(nomeInvalido, custoHora));
        ex.Message.ShouldContain("Nome do projeto");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(10000.01)]
    public void Nao_Deve_Aceitar_CustoHora_Invalido(decimal custoHoraInvalido)
    {
        // Arrange
        var nome = "Projeto Y";

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => new Projeto(nome, custoHoraInvalido));
        ex.Message.ShouldContain("Custo por hora");
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(9999.99)]
    [InlineData(10000)]
    public void Deve_Aceitar_CustoHora_Valido(decimal custoHoraValido)
    {
        // Arrange
        var nome = "Projeto Z";

        // Act
        var projeto = new Projeto(nome, custoHoraValido);

        // Assert
        projeto.ObterValorHora(DateTime.Now).ShouldBe(custoHoraValido);
    }

    [Fact]
    public void Deve_Selecionar_ValorHora_Correto_Por_Data()
    {
        // Arrange
        var projeto = new Projeto("Projeto A", 100m);
        projeto.DefinirValorHora(new DateTime(2025, 1, 1), new DateTime(2025, 12, 31, 23, 59, 59), 120m);

        // Act
        var valor2024 = projeto.ObterValorHora(new DateOnly(2024, 6, 1));
        var valor2025 = projeto.ObterValorHora(new DateOnly(2025, 6, 1));

        // Assert
        valor2024.ShouldBe(100m);
        valor2025.ShouldBe(120m);
    }

    [Fact]
    public void Nao_Deve_Permitir_Sobreposicao_De_Vigencias()
    {
        // Arrange
        var projeto = new Projeto("Projeto B", 100m);

        // Act
        projeto.DefinirValorHora(new DateTime(2026, 1, 1), new DateTime(2026, 6, 30, 23, 59, 59), 130m);

        // Assert
        var ex = Should.Throw<InvalidOperationException>(() =>
            projeto.DefinirValorHora(new DateTime(2026, 6, 1), new DateTime(2026, 12, 31, 23, 59, 59), 140m));

        ex.Message.ShouldContain("valor hora");
    }

    [Fact]
    public void Deve_Aceitar_Vigencia_Sem_DataFim()
    {
        // Arrange
        var projeto = new Projeto("Projeto C", 100m);

        // Act
        projeto.DefinirValorHora(new DateTime(2027, 1, 1), null, 150m);
        var valor = projeto.ObterValorHora(new DateOnly(2030, 1, 1));

        // Assert
        valor.ShouldBe(150m);
    }
}
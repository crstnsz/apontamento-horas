using System;
using Shouldly;
using Xunit;

namespace Apontamento.Domain;

public class DiaTrabalhadoTest
{
    [Fact]
    public void Deve_Criar_Dia_Trabalhado_Valido()
    {
        // Arrange
        var data = new DateOnly(2026, 2, 10);

        // Act
        var dia = new DiaTrabalhado(data);

        // Assert
        dia.Data.ShouldBe(data);
        dia.Periodos.ShouldBeEmpty();
        dia.TotalHoras.ShouldBe(TimeSpan.Zero);
        dia.ValorTotal.ShouldBe(0m);
    }

    [Fact]
    public void Nao_Deve_Aceitar_Data_Default()
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => new DiaTrabalhado(default));
        ex.Message.ShouldContain("Data do dia trabalhado");
    }

    [Fact]
    public void Deve_Adicionar_Periodos_E_Calcular_Totais()
    {
        // Arrange
        var projeto = new Projeto("Projeto X", 100m);
        projeto.DefinirValorHora(new DateTime(2026, 1, 1), null, 120m);

        var dia = new DiaTrabalhado(new DateOnly(2026, 2, 10));

        // Act
        dia.AdicionarPeriodo("Serviço 1", projeto, new TimeOnly(8, 0), new TimeOnly(10, 0));
        dia.AdicionarPeriodo("Serviço 2", projeto, new TimeOnly(10, 0), new TimeOnly(11, 0));

        // Assert
        dia.Periodos.Count.ShouldBe(2);
        dia.TotalHoras.ShouldBe(TimeSpan.FromHours(3));
        dia.ValorTotal.ShouldBe(360m);
    }

    [Fact]
    public void Nao_Deve_Aceitar_Periodo_Nulo()
    {
        // Arrange
        var dia = new DiaTrabalhado(new DateOnly(2026, 2, 10));

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => dia.AdicionarPeriodo(null!));
    }
}

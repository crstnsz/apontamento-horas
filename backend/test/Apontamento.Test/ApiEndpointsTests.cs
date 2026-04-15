using System.Net;
using System.Net.Http.Json;
using Apontamento.Domain;
using Apontamento.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;

namespace Apontamento.Api.Tests;

public class ApiEndpointsTests
{
    [Fact]
    public async Task Deve_Criar_E_Listar_Projeto()
    {
        var projetoRepository = new ProjetoRepositoryFake();
        var apontamentoRepository = new ApontamentoRepositoryFake();

        await using var factory = new CustomWebApplicationFactory(projetoRepository, apontamentoRepository);
        var client = factory.CreateClient();

        var payload = new
        {
            nome = "Projeto API Test",
            valoresHora = new[]
            {
                new { inicio = new DateTime(2026, 1, 1), fim = (DateTime?)null, valorHora = 120m }
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/projetos", payload);

        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjetoResponse>();
        created.ShouldNotBeNull();
        created.Nome.ShouldBe("Projeto API Test");

        var listagem = await client.GetFromJsonAsync<List<ProjetoResponse>>("/api/projetos");
        listagem.ShouldNotBeNull();
        listagem.Count.ShouldBe(1);
        listagem[0].Id.ShouldBe(created.Id);
    }

    [Fact]
    public async Task Deve_Criar_Apontamento_Para_Projeto_Existente()
    {
        var projeto = new Projeto("Projeto Base", 100m);
        var projetoRegistro = new ProjetoRegistro(Guid.NewGuid(), projeto);

        var projetoRepository = new ProjetoRepositoryFake([projetoRegistro]);
        var apontamentoRepository = new ApontamentoRepositoryFake();

        await using var factory = new CustomWebApplicationFactory(projetoRepository, apontamentoRepository);
        var client = factory.CreateClient();

        var payload = new
        {
            data = new DateOnly(2026, 2, 24),
            projetoId = projetoRegistro.Id,
            periodos = new[]
            {
                new { inicio = new TimeOnly(8, 0), fim = new TimeOnly(12, 0), descricao = "Implementação" },
                new { inicio = new TimeOnly(13, 0), fim = new TimeOnly(17, 30), descricao = "Revisão" }
            }
        };

        var response = await client.PostAsJsonAsync("/api/apontamentos", payload);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var apontamentos = await client.GetFromJsonAsync<List<ApontamentoResponse>>("/api/apontamentos");
        apontamentos.ShouldNotBeNull();
        apontamentos.Count.ShouldBe(1);
        apontamentos[0].ProjetoId.ShouldBe(projetoRegistro.Id);
        apontamentos[0].TotalHoras.ShouldBe(8.5m);
    }

    [Fact]
    public async Task Deve_Calcular_Consulta_Por_Periodo()
    {
        var projeto = new Projeto("Projeto Consulta", 100m);
        var projetoRegistro = new ProjetoRegistro(Guid.NewGuid(), projeto);

        var dia = new DiaTrabalhado(new DateOnly(2026, 2, 24));
        dia.AdicionarPeriodo("Atividade", projeto, new TimeOnly(8, 0), new TimeOnly(12, 0));

        var apontamentoRegistro = new ApontamentoRegistro(Guid.NewGuid(), dia, projetoRegistro.Id);

        var projetoRepository = new ProjetoRepositoryFake([projetoRegistro]);
        var apontamentoRepository = new ApontamentoRepositoryFake([apontamentoRegistro]);

        await using var factory = new CustomWebApplicationFactory(projetoRepository, apontamentoRepository);
        var client = factory.CreateClient();

        var consulta = await client.GetFromJsonAsync<ConsultaResponse>("/api/consultas?inicio=2026-02-24&fim=2026-02-24");

        consulta.ShouldNotBeNull();
        consulta.HorasPrevistas.ShouldBe(8.5m);
        consulta.HorasRealizadas.ShouldBe(4m);
        consulta.Diferenca.ShouldBe(4.5m);
    }
}

internal sealed class CustomWebApplicationFactory(
    IProjetoRepository projetoRepository,
    IApontamentoRepository apontamentoRepository) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IProjetoRepository>();
            services.RemoveAll<IApontamentoRepository>();

            services.AddSingleton(projetoRepository);
            services.AddSingleton(apontamentoRepository);
        });
    }
}

internal sealed class ProjetoRepositoryFake(IEnumerable<ProjetoRegistro>? seed = null) : IProjetoRepository
{
    private readonly List<ProjetoRegistro> _projetos = seed?.ToList() ?? [];

    public Task<IReadOnlyCollection<ProjetoRegistro>> ListarAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyCollection<ProjetoRegistro>>(_projetos.ToList());

    public Task<ProjetoRegistro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_projetos.FirstOrDefault(p => p.Id == id));

    public Task AdicionarAsync(ProjetoRegistro projeto, CancellationToken cancellationToken = default)
    {
        _projetos.Add(projeto);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(ProjetoRegistro projeto, CancellationToken cancellationToken = default)
    {
        var index = _projetos.FindIndex(p => p.Id == projeto.Id);
        if (index >= 0)
        {
            _projetos[index] = projeto;
        }
        return Task.CompletedTask;
    }

    public Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _projetos.RemoveAll(p => p.Id == id);
        return Task.CompletedTask;
    }
}

internal sealed class ApontamentoRepositoryFake(IEnumerable<ApontamentoRegistro>? seed = null) : IApontamentoRepository
{
    private readonly List<ApontamentoRegistro> _apontamentos = seed?.ToList() ?? [];

    public Task<IReadOnlyCollection<ApontamentoRegistro>> ListarAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyCollection<ApontamentoRegistro>>(_apontamentos.ToList());

    public Task<IReadOnlyCollection<ApontamentoRegistro>> ListarPorPeriodoAsync(DateOnly inicio, DateOnly fim, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyCollection<ApontamentoRegistro>>(
            _apontamentos.Where(a => a.Dia.Data >= inicio && a.Dia.Data <= fim).ToList());

    public Task<ApontamentoRegistro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_apontamentos.FirstOrDefault(a => a.Id == id));

    public Task AdicionarAsync(ApontamentoRegistro apontamento, CancellationToken cancellationToken = default)
    {
        _apontamentos.Add(apontamento);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(ApontamentoRegistro apontamento, CancellationToken cancellationToken = default)
    {
        var index = _apontamentos.FindIndex(a => a.Id == apontamento.Id);
        if (index >= 0)
        {
            _apontamentos[index] = apontamento;
        }
        return Task.CompletedTask;
    }

    public Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _apontamentos.RemoveAll(a => a.Id == id);
        return Task.CompletedTask;
    }
}

internal sealed record ProjetoResponse(Guid Id, string Nome, List<ValorHoraResponse> ValoresHora);
internal sealed record ValorHoraResponse(DateTime Inicio, DateTime? Fim, decimal ValorHora);
internal sealed record ApontamentoResponse(Guid Id, DateOnly Data, Guid ProjetoId, decimal TotalHoras, decimal ValorTotal, List<PeriodoResponse> Periodos);
internal sealed record PeriodoResponse(TimeOnly Inicio, TimeOnly Fim, string Descricao);
internal sealed record ConsultaResponse(decimal HorasPrevistas, decimal HorasRealizadas, decimal Diferenca);

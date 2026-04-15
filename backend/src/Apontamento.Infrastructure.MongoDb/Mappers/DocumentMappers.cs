using System;
using System.Globalization;
using System.Linq;
using Apontamento.Domain;
using Apontamento.Domain.Repositories;
using Apontamento.Infrastructure.MongoDb.Documents;

namespace Apontamento.Infrastructure.MongoDb.Mappers;

internal static class DocumentMappers
{
    private const string TimeFormat = "HH:mm:ss";

    public static ProjetoDocument ToDocument(this ProjetoRegistro registro)
        => new()
        {
            Id = registro.Id,
            Nome = registro.Projeto.Nome,
            ValoresHora = registro.Projeto.ValoresHora
                .Select(v => new ValorHoraVigenciaDocument
                {
                    Inicio = v.Inicio,
                    Fim = v.Fim,
                    ValorHora = v.ValorHora
                })
                .ToList()
        };

    public static ProjetoRegistro ToDomain(this ProjetoDocument document)
    {
        var vigencias = document.ValoresHora.OrderBy(v => v.Inicio).ToList();

        if (vigencias.Count == 0)
        {
            throw new InvalidOperationException("Projeto sem valor hora cadastrado.");
        }

        var projeto = new Projeto(document.Nome, vigencias[0].ValorHora);
        for (var i = 1; i < vigencias.Count; i++)
        {
            projeto.DefinirValorHora(vigencias[i].Inicio, vigencias[i].Fim, vigencias[i].ValorHora);
        }

        return new ProjetoRegistro(document.Id, projeto);
    }

    public static ApontamentoDocument ToDocument(this ApontamentoRegistro registro)
        => new()
        {
            Id = registro.Id,
            Data = registro.Dia.Data.ToDateTime(TimeOnly.MinValue),
            ProjetoId = registro.ProjetoId,
            Periodos = registro.Dia.Periodos.Select(p => new PeriodoDocument
            {
                Inicio = p.Inicio.ToString(TimeFormat, CultureInfo.InvariantCulture),
                Fim = p.Fim.ToString(TimeFormat, CultureInfo.InvariantCulture),
                DescricaoServico = p.DescricaoServico,
                Projeto = new ProjetoRegistro(Guid.Empty, p.Projeto).ToDocument()
            }).ToList()
        };

    public static ApontamentoRegistro ToDomain(this ApontamentoDocument document)
    {
        var dia = new DiaTrabalhado(DateOnly.FromDateTime(document.Data));
        foreach (var periodo in document.Periodos)
        {
            var projeto = periodo.Projeto.ToDomain().Projeto;
            dia.AdicionarPeriodo(
                periodo.DescricaoServico,
                projeto,
                TimeOnly.ParseExact(periodo.Inicio, TimeFormat, CultureInfo.InvariantCulture),
                TimeOnly.ParseExact(periodo.Fim, TimeFormat, CultureInfo.InvariantCulture));
        }

        return new ApontamentoRegistro(document.Id, dia, document.ProjetoId);
    }
}

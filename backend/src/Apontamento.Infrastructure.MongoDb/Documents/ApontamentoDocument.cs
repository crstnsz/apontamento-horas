using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Apontamento.Infrastructure.MongoDb.Documents;

public sealed class ApontamentoDocument
{
    [BsonId]
    public Guid Id { get; set; }

    public DateTime Data { get; set; }

    public Guid ProjetoId { get; set; }

    public List<PeriodoDocument> Periodos { get; set; } = [];
}

public sealed class PeriodoDocument
{
    public string Inicio { get; set; } = string.Empty;
    public string Fim { get; set; } = string.Empty;
    public string DescricaoServico { get; set; } = string.Empty;
    public ProjetoDocument Projeto { get; set; } = new();
}

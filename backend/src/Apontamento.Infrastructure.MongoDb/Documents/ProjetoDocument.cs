using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Apontamento.Infrastructure.MongoDb.Documents;

public sealed class ProjetoDocument
{
    [BsonId]
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public List<ValorHoraVigenciaDocument> ValoresHora { get; set; } = [];
}

public sealed class ValorHoraVigenciaDocument
{
    public DateTime Inicio { get; set; }
    public DateTime Fim { get; set; }
    public decimal ValorHora { get; set; }
}

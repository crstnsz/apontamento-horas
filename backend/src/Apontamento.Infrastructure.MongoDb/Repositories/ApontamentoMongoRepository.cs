using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apontamento.Domain.Repositories;
using Apontamento.Infrastructure.MongoDb.Documents;
using Apontamento.Infrastructure.MongoDb.Mappers;
using MongoDB.Driver;

namespace Apontamento.Infrastructure.MongoDb.Repositories;

public sealed class ApontamentoMongoRepository : IApontamentoRepository
{
    private readonly IMongoCollection<ApontamentoDocument> _collection;

    public ApontamentoMongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<ApontamentoDocument>(collectionName);
    }

    public async Task<IReadOnlyCollection<ApontamentoRegistro>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var documentos = await _collection.Find(FilterDefinition<ApontamentoDocument>.Empty)
            .ToListAsync(cancellationToken);

        return documentos.Select(d => d.ToDomain()).ToList();
    }

    public async Task<IReadOnlyCollection<ApontamentoRegistro>> ListarPorPeriodoAsync(DateOnly inicio, DateOnly fim, CancellationToken cancellationToken = default)
    {
        var inicioDataHora = inicio.ToDateTime(TimeOnly.MinValue);
        var fimDataHora = fim.ToDateTime(TimeOnly.MaxValue);

        var documentos = await _collection
            .Find(a => a.Data >= inicioDataHora && a.Data <= fimDataHora)
            .ToListAsync(cancellationToken);

        return documentos.Select(d => d.ToDomain()).ToList();
    }

    public async Task<ApontamentoRegistro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var documento = await _collection.Find(a => a.Id == id).FirstOrDefaultAsync(cancellationToken);
        return documento?.ToDomain();
    }

    public Task AdicionarAsync(ApontamentoRegistro apontamento, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(apontamento.ToDocument(), cancellationToken: cancellationToken);

    public Task AtualizarAsync(ApontamentoRegistro apontamento, CancellationToken cancellationToken = default)
        => _collection.ReplaceOneAsync(a => a.Id == apontamento.Id, apontamento.ToDocument(), cancellationToken: cancellationToken);

    public Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
        => _collection.DeleteOneAsync(a => a.Id == id, cancellationToken);
}

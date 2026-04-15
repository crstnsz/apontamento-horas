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

public sealed class ProjetoMongoRepository : IProjetoRepository
{
    private readonly IMongoCollection<ProjetoDocument> _collection;

    public ProjetoMongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<ProjetoDocument>(collectionName);
    }

    public async Task<IReadOnlyCollection<ProjetoRegistro>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var documentos = await _collection.Find(FilterDefinition<ProjetoDocument>.Empty)
            .ToListAsync(cancellationToken);

        return documentos.Select(d => d.ToDomain()).ToList();
    }

    public async Task<ProjetoRegistro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var documento = await _collection.Find(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
        return documento?.ToDomain();
    }

    public Task AdicionarAsync(ProjetoRegistro projeto, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(projeto.ToDocument(), cancellationToken: cancellationToken);

    public Task AtualizarAsync(ProjetoRegistro projeto, CancellationToken cancellationToken = default)
        => _collection.ReplaceOneAsync(p => p.Id == projeto.Id, projeto.ToDocument(), cancellationToken: cancellationToken);

    public Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
        => _collection.DeleteOneAsync(p => p.Id == id, cancellationToken);
}

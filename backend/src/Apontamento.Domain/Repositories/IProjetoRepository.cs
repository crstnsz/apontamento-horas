using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apontamento.Domain.Repositories;

public interface IProjetoRepository
{
    Task<IReadOnlyCollection<ProjetoRegistro>> ListarAsync(CancellationToken cancellationToken = default);
    Task<ProjetoRegistro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(ProjetoRegistro projeto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(ProjetoRegistro projeto, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}

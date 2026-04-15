using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apontamento.Domain.Repositories;

public interface IApontamentoRepository
{
    Task<IReadOnlyCollection<ApontamentoRegistro>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ApontamentoRegistro>> ListarPorPeriodoAsync(DateOnly inicio, DateOnly fim, CancellationToken cancellationToken = default);
    Task<ApontamentoRegistro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(ApontamentoRegistro apontamento, CancellationToken cancellationToken = default);
    Task AtualizarAsync(ApontamentoRegistro apontamento, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}

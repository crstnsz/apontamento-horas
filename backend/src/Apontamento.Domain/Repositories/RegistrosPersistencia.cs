using System;

namespace Apontamento.Domain.Repositories;

public sealed record ProjetoRegistro(Guid Id, Projeto Projeto);

public sealed record ApontamentoRegistro(Guid Id, DiaTrabalhado Dia, Guid ProjetoId);

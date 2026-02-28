using FraudStream.Application.Common;
using FraudStream.Application.DTOs;
using FraudStream.Domain.Enums;
using MediatR;

namespace FraudStream.Application.Queries.GetTransactionsList
{
    /// <summary>
    /// Query paginada para listagem de transações com filtros opcionais.
    /// Usada pela tabela ao vivo do dashboard Grafana e pela interface de revisão manual.
    /// </summary>
    public sealed record GetTransactionsListQuery(
        int Page = 1,
        int PageSize = 20,
        DecisionStatus? Status = null,
        string? CardId = null,
        DateTime? From = null,
        DateTime? To = null
    ) : IRequest<Result<PagedResult<TransactionDto>>>;
}

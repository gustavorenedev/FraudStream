using FraudStream.Application.Common;
using FraudStream.Application.DTOs;
using MediatR;

namespace FraudStream.Application.Queries.GetTransactionById
{
    /// <summary>
    /// Query do Read side do CQRS.
    /// Retorna os detalhes completos de uma transação com sua decisão de fraude.
    /// </summary>
    public sealed record GetTransactionByIdQuery(string TransactionId)
        : IRequest<Result<TransactionDto>>;
}

using FraudStream.Application.Common;
using FraudStream.Application.DTOs;
using FraudStream.Domain.Interfaces;
using MediatR;

namespace FraudStream.Application.Queries.GetTransactionsList
{
    /// <summary>
    /// Retorna lista paginada de transações com filtros.
    /// A implementação no repositório usa Dapper com COUNT(*) OVER() para paginação eficiente.
    /// </summary>
    public sealed class GetTransactionsListHandler
    : IRequestHandler<GetTransactionsListQuery, Result<PagedResult<TransactionDto>>>
    {
        private readonly ITransactionRepository _repository;

        public GetTransactionsListHandler(ITransactionRepository repository)
            => _repository = repository;

        public async Task<Result<PagedResult<TransactionDto>>> Handle(
            GetTransactionsListQuery query,
            CancellationToken ct)
        {
            var to = query.To ?? DateTime.UtcNow;
            var from = query.From ?? to.AddHours(-24);

            var paged = await _repository.GetPagedAsync(
                query.Page, query.PageSize,
                query.Status, query.CardId,
                from, to, ct);

            var dtos = paged.Items.Select(t => t.ToDto()).ToList();

            return Result<PagedResult<TransactionDto>>.Success(
                new PagedResult<TransactionDto>(dtos, query.Page, query.PageSize, paged.TotalCount));
        }
    }
}

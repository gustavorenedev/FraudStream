using FraudStream.Application.Common;
using FraudStream.Application.DTOs;
using FraudStream.Domain.Interfaces;
using MediatR;

namespace FraudStream.Application.Queries.GetFraudSummary
{
    /// <summary>
    /// Retorna o resumo agregado de decisões de fraude para o período informado.
    /// A query usa uma view otimizada no PostgreSQL via Dapper — não carrega entidades.
    /// </summary>
    public sealed class GetFraudSummaryHandler
        : IRequestHandler<GetFraudSummaryQuery, Result<FraudSummaryDto>>
    {
        private readonly IFraudDecisionRepository _repository;

        public GetFraudSummaryHandler(IFraudDecisionRepository repository)
            => _repository = repository;

        public async Task<Result<FraudSummaryDto>> Handle(
            GetFraudSummaryQuery query,
            CancellationToken ct)
        {
            var to = query.To ?? DateTime.UtcNow;
            var from = query.From ?? to.AddHours(-24);

            var summary = await _repository.GetSummaryAsync(from, to, ct);

            return Result<FraudSummaryDto>.Success(summary.ToDto());
        }
    }
}

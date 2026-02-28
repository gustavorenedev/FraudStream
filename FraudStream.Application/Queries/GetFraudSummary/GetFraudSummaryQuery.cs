using FraudStream.Application.Common;
using FraudStream.Application.DTOs;
using MediatR;

namespace FraudStream.Application.Queries.GetFraudSummary
{
    /// <summary>
    /// Query que alimenta o dashboard do Grafana.
    /// Retorna métricas agregadas de fraude para um período de tempo.
    /// Período padrão: últimas 24 horas.
    /// </summary>
    public sealed record GetFraudSummaryQuery(
        DateTime? From = null,
        DateTime? To = null
    ) : IRequest<Result<FraudSummaryDto>>;
}

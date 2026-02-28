using FraudStream.Application.Common;
using FraudStream.Domain.Enums;
using MediatR;

namespace FraudStream.Application.Commands.ManualDecision
{
    /// <summary>
    /// Command disparado quando um analista conclui a revisão manual de uma transação.
    /// Só válido para transações com DecisionStatus.Review.
    /// </summary>
    public sealed record ManualDecisionCommand(
        string TransactionId,
        string AnalystId,
        DecisionStatus FinalDecision,
        string Notes
    ) : IRequest<Result>;
}

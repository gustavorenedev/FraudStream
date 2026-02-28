using FluentValidation;
using FraudStream.Application.Commands.ManualDecision;
using FraudStream.Domain.Enums;

namespace FraudStream.Application.Validators
{
    public sealed class ManualDecisionCommandValidator : AbstractValidator<ManualDecisionCommand>
    {
        public ManualDecisionCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("TransactionId é obrigatório.")
                .Must(id => id.StartsWith("txn_")).WithMessage("TransactionId inválido.");

            RuleFor(x => x.AnalystId)
                .NotEmpty().WithMessage("AnalystId é obrigatório.");

            RuleFor(x => x.FinalDecision)
                .Must(d => d is DecisionStatus.Approved or DecisionStatus.Blocked or DecisionStatus.FraudConfirmed)
                .WithMessage("Decisão final deve ser Approved, Blocked ou FraudConfirmed.");

            RuleFor(x => x.Notes)
                .NotEmpty().WithMessage("Notas de revisão são obrigatórias para auditoria.")
                .MinimumLength(10).WithMessage("Notas devem ter ao menos 10 caracteres.")
                .MaximumLength(1000).WithMessage("Notas não podem ultrapassar 1000 caracteres.");
        }
    }
}

using FluentValidation;
using FraudStream.Application.Commands.ProcessTransaction;

namespace FraudStream.Application.Validators
{
    /// <summary>
    /// Validação do payload antes de chegar no Handler.
    /// Erros aqui retornam 422 antes de qualquer lógica de domínio ser executada.
    /// </summary>
    public sealed class ProcessTransactionCommandValidator
        : AbstractValidator<ProcessTransactionCommand>
    {
        public ProcessTransactionCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("O valor da transação deve ser maior que zero.")
                .LessThanOrEqualTo(1_000_000).WithMessage("Valor acima do limite permitido.");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Moeda é obrigatória.")
                .Length(3).WithMessage("Moeda deve ter exatamente 3 letras (ISO 4217).")
                .Matches("^[A-Z]{3}$").WithMessage("Moeda deve conter apenas letras maiúsculas.");

            RuleFor(x => x.CardId)
                .NotEmpty().WithMessage("CardId é obrigatório.")
                .MaximumLength(64).WithMessage("CardId muito longo.");

            RuleFor(x => x.CardLastFourDigits)
                .NotEmpty().WithMessage("Últimos 4 dígitos do cartão são obrigatórios.")
                .Length(4).WithMessage("Deve conter exatamente 4 dígitos.")
                .Matches("^[0-9]{4}$").WithMessage("Deve conter apenas dígitos.");

            RuleFor(x => x.CardIssuedAt)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Data de emissão do cartão não pode ser futura.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("DeviceId é obrigatório.")
                .MaximumLength(128).WithMessage("DeviceId muito longo.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("País é obrigatório.")
                .Length(2).WithMessage("País deve ter exatamente 2 letras (ISO 3166-1 alpha-2).")
                .Matches("^[A-Z]{2}$").WithMessage("País deve conter apenas letras maiúsculas.");

            RuleFor(x => x.IpAddress)
                .Matches(@"^(\d{1,3}\.){3}\d{1,3}$|^([0-9a-fA-F:]+)$")
                .When(x => x.IpAddress is not null)
                .WithMessage("Endereço IP inválido.");

            RuleFor(x => x.MerchantId)
                .NotEmpty().WithMessage("MerchantId é obrigatório.");

            RuleFor(x => x.MerchantName)
                .NotEmpty().WithMessage("Nome do merchant é obrigatório.")
                .MaximumLength(200).WithMessage("Nome do merchant muito longo.");

            RuleFor(x => x.MerchantCategory)
                .NotEmpty().WithMessage("Categoria do merchant é obrigatória.");
        }
    }
}

using FraudStream.Application.Common;
using MediatR;

namespace FraudStream.Application.Commands.ProcessTransaction
{
    /// <summary>
    /// Command disparado pelo TransactionsController quando recebe um POST /api/transactions.
    /// Também pode ser disparado diretamente pelo Simulator sem passar pela API.
    /// Retorna o TransactionId gerado para que o cliente possa consultar o resultado depois.
    /// </summary>
    public sealed record ProcessTransactionCommand(
        decimal Amount,
        string Currency,
        string CardId,
        string CardLastFourDigits,
        DateTime CardIssuedAt,
        string DeviceId,
        string Country,
        string? IpAddress,
        string MerchantId,
        string MerchantName,
        string MerchantCategory
    ) : IRequest<Result<string>>;
}

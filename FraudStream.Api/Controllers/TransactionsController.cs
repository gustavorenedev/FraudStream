using FraudStream.Application.Commands.ProcessTransaction;
using FraudStream.Application.Queries.GetTransactionById;
using FraudStream.Application.Queries.GetTransactionsList;
using FraudStream.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FraudStream.Api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Produces("application/json")]
    public sealed class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator) => _mediator = mediator;

        /// <summary>Submete uma nova transação para análise de fraude.</summary>
        /// <remarks>
        /// Retorna 202 Accepted — a análise ocorre de forma assíncrona via pipeline de eventos.
        /// Use o transactionId retornado para consultar o resultado via GET.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(CreateTransactionResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Create(
            [FromBody] CreateTransactionRequest request,
            CancellationToken ct)
        {
            var command = new ProcessTransactionCommand(
                Amount: request.Amount,
                Currency: request.Currency,
                CardId: request.CardId,
                CardLastFourDigits: request.CardLastFourDigits,
                CardIssuedAt: request.CardIssuedAt,
                DeviceId: request.DeviceId,
                Country: request.Country,
                IpAddress: request.IpAddress,
                MerchantId: request.MerchantId,
                MerchantName: request.MerchantName,
                MerchantCategory: request.MerchantCategory
            );

            var result = await _mediator.Send(command, ct);

            return result.Match(
                onSuccess: transactionId => Accepted(
                    uri: $"/api/transactions/{transactionId}",
                    value: new CreateTransactionResponse(transactionId)),
                onFailure: error => (IActionResult)UnprocessableEntity(new { error }));
        }

        /// <summary>Retorna os detalhes de uma transação com sua decisão de fraude.</summary>
        [HttpGet("{transactionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string transactionId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetTransactionByIdQuery(transactionId), ct);

            return result.Match(
                onSuccess: dto => Ok(dto),
                onFailure: _ => (IActionResult)NotFound());
        }

        /// <summary>Listagem paginada de transações com filtros opcionais.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] DecisionStatus? status = null,
            [FromQuery] string? cardId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(
                new GetTransactionsListQuery(page, pageSize, status, cardId, from, to), ct);

            return result.Match(
                onSuccess: paged => Ok(paged),
                onFailure: error => (IActionResult)BadRequest(new { error }));
        }
    }

    // ── Request / Response records ─────────────────────────────────────────────────

    public sealed record CreateTransactionRequest(
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
    );

    public sealed record CreateTransactionResponse(string TransactionId);
}

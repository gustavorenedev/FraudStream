using FraudStream.Application.Commands.ManualDecision;
using FraudStream.Application.Queries.GetFraudSummary;
using FraudStream.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FraudStream.Api.Controllers
{
    [ApiController]
    [Route("api/fraud")]
    [Produces("application/json")]
    public sealed class FraudSummaryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FraudSummaryController(IMediator mediator) => _mediator = mediator;

        /// <summary>Retorna métricas agregadas de fraude para o período informado.</summary>
        /// <remarks>Período padrão: últimas 24 horas. Usado pelo dashboard do Grafana.</remarks>
        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetFraudSummaryQuery(from, to), ct);

            return result.Match(
                onSuccess: dto => Ok(dto),
                onFailure: error => (IActionResult)BadRequest(new { error }));
        }

        /// <summary>Aplica a decisão manual de um analista sobre uma transação em revisão.</summary>
        [HttpPost("{transactionId}/review")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ApplyManualReview(
            string transactionId,
            [FromBody] ManualReviewRequest request,
            CancellationToken ct)
        {
            var command = new ManualDecisionCommand(
                TransactionId: transactionId,
                AnalystId: request.AnalystId,
                FinalDecision: request.FinalDecision,
                Notes: request.Notes);

            var result = await _mediator.Send(command, ct);

            return result.IsSuccess ? NoContent()
                                    : UnprocessableEntity(new { error = result.Error });
        }
    }

    public sealed record ManualReviewRequest(
        string AnalystId,
        DecisionStatus FinalDecision,
        string Notes
    );
}

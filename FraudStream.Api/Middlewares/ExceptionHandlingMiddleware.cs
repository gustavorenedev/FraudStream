using FraudStream.Application.Exceptions;
using FraudStream.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace FraudStream.Api.Middlewares
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, title, errors) = exception switch
            {
                NotFoundException nfe =>
                    (HttpStatusCode.NotFound, nfe.Message, (object?)null),

                Application.Exceptions.ValidationException ve =>
                    (HttpStatusCode.UnprocessableEntity, "Erros de validação.", (object?)ve.Errors),

                DomainException de =>
                    (HttpStatusCode.BadRequest, de.Message, null),

                _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno.", null)
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Erro não tratado: {Message}", exception.Message);

            var response = new
            {
                type = "https://tools.ietf.org/html/rfc7807",
                title,
                status = (int)statusCode,
                errors,
                traceId = context.TraceIdentifier
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        }
    }
}

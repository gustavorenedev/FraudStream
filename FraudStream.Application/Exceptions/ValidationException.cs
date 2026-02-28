namespace FraudStream.Application.Exceptions
{
    /// <summary>
    /// Lançada quando a validação do comando/query falha (FluentValidation).
    /// Traduzida para 422 Unprocessable Entity pelo ExceptionHandlingMiddleware.
    /// </summary>
    public sealed class ValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("Um ou mais erros de validação ocorreram.")
        {
            Errors = errors.AsReadOnly();
        }
    }
}

namespace FraudStream.Application.Exceptions
{
    /// <summary>
    /// Lançada quando um recurso solicitado não é encontrado.
    /// Traduzida para 404 Not Found pelo ExceptionHandlingMiddleware da API.
    /// </summary>
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string resourceName, string key)
            : base($"{resourceName} com identificador '{key}' não foi encontrado.") { }
    }

}

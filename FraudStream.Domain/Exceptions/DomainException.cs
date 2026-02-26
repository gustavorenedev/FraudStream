namespace FraudStream.Domain.Exceptions
{
    /// <summary>
    /// Exceção lançada quando uma regra de negócio do domínio é violada.
    /// Deve ser tratada na camada de Application e traduzida para respostas HTTP adequadas (400/422).
    /// Nunca deve vazar para a infraestrutura como exceção não tratada.
    /// </summary>
    public sealed class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }

        public DomainException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

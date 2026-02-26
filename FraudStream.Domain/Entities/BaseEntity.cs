namespace FraudStream.Domain.Entities
{
    /// <summary>
    /// Classe base que toda entidade herda. Tem três responsabilidades:
    /// Gerar um Id único ao nascer.
    /// Controlar CreatedAt e UpdatedAt.
    /// Manter uma lista interna de domain events que foram disparados durante a vida daquela entidade.
    /// </summary>
    public class BaseEntity
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents()
            => _domainEvents.Clear();

        protected void SetUpdatedAt()
            => UpdatedAt = DateTime.UtcNow;
    }
}

using FraudStream.Application.Common;
using FraudStream.Application.Interfaces;
using FraudStream.Domain.Entities;
using FraudStream.Domain.Interfaces;
using FraudStream.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FraudStream.Application.Commands.ProcessTransaction
{
    /// <summary>
    /// Orquestra a criação de uma Transaction e a publicação do evento no broker.
    /// 
    /// Fluxo:
    ///   1. Monta os Value Objects validando os dados de entrada
    ///   2. Cria a Transaction via factory (que dispara o TransactionCreatedEvent internamente)
    ///   3. Persiste a Transaction no banco com status Received
    ///   4. Publica os domain events coletados (TransactionCreatedEvent → RabbitMQ)
    ///   5. Retorna o TransactionId para o cliente — a análise ocorre de forma assíncrona
    /// </summary>
    public sealed class ProcessTransactionHandler
        : IRequestHandler<ProcessTransactionCommand, Result<string>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<ProcessTransactionHandler> _logger;

        public ProcessTransactionHandler(
            ITransactionRepository transactionRepository,
            IEventPublisher eventPublisher,
            ILogger<ProcessTransactionHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(
            ProcessTransactionCommand command,
            CancellationToken ct)
        {
            try
            {
                // 1. Monta Value Objects — DomainException lança se dados inválidos
                var transactionId = TransactionId.New();
                var amount = new Money(command.Amount, command.Currency);
                var card = new CardInfo(command.CardId, command.CardLastFourDigits, command.CardIssuedAt);
                var device = new DeviceInfo(command.DeviceId, command.Country, command.IpAddress);
                var merchant = new MerchantInfo(command.MerchantId, command.MerchantName, command.MerchantCategory);

                // 2. Cria Transaction — TransactionCreatedEvent é adicionado internamente
                var transaction = Transaction.Create(transactionId, amount, card, device, merchant);

                // 3. Persiste com status Received
                await _transactionRepository.AddAsync(transaction, ct);

                // 4. Publica os domain events coletados para o RabbitMQ
                await _eventPublisher.PublishDomainEventsAsync(transaction.DomainEvents, ct);
                transaction.ClearDomainEvents();

                _logger.LogInformation(
                    "Transação {TransactionId} recebida e publicada no broker. Valor: {Amount} {Currency}",
                    transactionId.Value, command.Amount, command.Currency);

                // 5. Retorna o ID — cliente usa para consultar o resultado via GET
                return Result<string>.Success(transactionId.Value);
            }
            catch (Domain.Exceptions.DomainException ex)
            {
                _logger.LogWarning("Dados inválidos ao processar transação: {Error}", ex.Message);
                return Result<string>.Failure(ex.Message);
            }
        }
    }
}

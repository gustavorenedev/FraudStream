using FraudStream.Application.Common;
using FraudStream.Application.DTOs;
using FraudStream.Application.Exceptions;
using FraudStream.Domain.Interfaces;
using MediatR;

namespace FraudStream.Application.Queries.GetTransactionById
{
    /// <summary>
    /// Busca uma transação pelo ID e mapeia para DTO.
    /// Usa Dapper na camada de Infrastructure para leitura otimizada.
    /// </summary>
    public sealed class GetTransactionByIdHandler
        : IRequestHandler<GetTransactionByIdQuery, Result<TransactionDto>>
    {
        private readonly ITransactionRepository _repository;

        public GetTransactionByIdHandler(ITransactionRepository repository)
            => _repository = repository;

        public async Task<Result<TransactionDto>> Handle(
            GetTransactionByIdQuery query,
            CancellationToken ct)
        {
            var transaction = await _repository.GetByTransactionIdAsync(query.TransactionId, ct);

            if (transaction is null)
                throw new NotFoundException(nameof(transaction), query.TransactionId);

            return Result<TransactionDto>.Success(transaction.ToDto());
        }
    }

}

namespace FraudStream.Domain.Enums
{
    public enum TransactionStatus
    {
        /// <summary>Recebida pela API, aguardando processamento.</summary>
        Received = 0,

        /// <summary>Em análise pelo Fraud Engine.</summary>
        Processing = 1,

        /// <summary>Análise concluída — resultado disponível em FraudDecision.</summary>
        Analyzed = 2,

        /// <summary>Falha no processamento — mensagem encaminhada para DLQ.</summary>
        Failed = 3
    }
}

namespace FraudStream.Domain.Enums
{
    public enum DecisionStatus
    {
        /// <summary>Transação ainda não foi analisada pelo engine.</summary>
        Pending = 0,

        /// <summary>Score 0–30. Transação aprovada automaticamente.</summary>
        Approved = 1,

        /// <summary>Score 31–60. Enviada para fila de revisão manual.</summary>
        Review = 2,

        /// <summary>Score 61–100. Bloqueada preventivamente pelo engine.</summary>
        Blocked = 3,

        /// <summary>Score acima de 100. Fraude confirmada — ocorrência aberta.</summary>
        FraudConfirmed = 4
    }
}

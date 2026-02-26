namespace FraudStream.Domain.Enums
{
    public enum RuleType
    {
        HighValue = 1,
        Velocity = 2,
        UnusualCountry = 3,
        SuspiciousHour = 4,
        NewDevice = 5,
        BlockedMerchant = 6,
        NewCard = 7
    }
}

using FraudStream.Domain.Exceptions;

namespace FraudStream.Domain.ValueObjects
{
    /// <summary>
    /// Informações do dispositivo que originou a transação.
    /// Usado pelas regras NewDevice (RG-05) e UnusualCountry (RG-03).
    /// </summary>
    public sealed record DeviceInfo
    {
        public string DeviceId { get; }
        public string Country { get; }
        public string? IpAddress { get; }

        private DeviceInfo() { } // EF Core

        public DeviceInfo(string deviceId, string country, string? ipAddress = null)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new DomainException("DeviceId não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(country) || country.Length != 2)
                throw new DomainException("País inválido. Informe um código ISO 3166-1 alpha-2 (ex: BR, US).");

            if (ipAddress is not null && !IsValidIp(ipAddress))
                throw new DomainException($"IP inválido: {ipAddress}.");

            DeviceId = deviceId;
            Country = country.ToUpperInvariant();
            IpAddress = ipAddress;
        }

        private static bool IsValidIp(string ip)
            => System.Net.IPAddress.TryParse(ip, out _);

        public override string ToString() => $"Device={DeviceId} Country={Country}";
    }
}

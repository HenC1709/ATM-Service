namespace ATM2.Core
{
    /// <summary>
    /// Reglas de negocio configurables del cajero. Cambia los valores aquí
    /// en lugar de tener "números mágicos" repartidos por los servicios.
    /// </summary>
    public static class AtmSettings
    {
        // Seguridad
        public const int PinLength = 4;
        public const int MinPasswordLength = 6;
        public const int MaxPinAttempts = 3;
        public const int MaxLoginAttempts = 3;
        public const int SessionTimeoutMinutes = 5;

        // Tarjetas
        public const int CardValidityYears = 4;

        // Operaciones en cajero
        public const decimal WithdrawalMultiple = 100m;
        public const decimal MaxWithdrawalPerTransaction = 10_000m;
        public const decimal MaxDepositPerTransaction = 50_000m;

        // Créditos (deudas)
        public const decimal CreditInterestRate = 0.10m;
        public const decimal CreditAutoApprovalLimit = 10_000m;
        public const decimal MaxCreditAmount = 100_000m;
    }
}

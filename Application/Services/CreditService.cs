using ATM2.Core;
using ATM2.Core.Enums;
using ATM2.Core.Exceptions;
using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Interfaces.Services;
using ATM2.Core.Models;

namespace ATM2.Application.Services
{
    public class CreditService : ICreditService
    {
        private readonly ICreditRepository _credits;
        private readonly IAccountService _accounts;

        public CreditService(ICreditRepository credits, IAccountService accounts)
        {
            _credits = credits;
            _accounts = accounts;
        }

        public Credit? GetById(string id) => _credits.GetById(id);

        public List<Credit> GetByUserId(string userId) =>
            _credits.GetByUserId(userId).OrderByDescending(c => c.RequestedAt).ToList();

        public List<Credit> GetPendingDebts(string userId) =>
            GetByUserId(userId).Where(c => c.Status == CreditStatus.Approved && c.RemainingBalance > 0).ToList();

        public List<Credit> GetAll() =>
            _credits.GetAll().OrderByDescending(c => c.RequestedAt).ToList();

        public Credit Request(string userId, decimal amount)
        {
            if (amount <= 0)
                throw new AtmException("El monto debe ser mayor que cero.");
            if (amount > AtmSettings.MaxCreditAmount)
                throw new AtmException($"El monto máximo de crédito es {AtmSettings.MaxCreditAmount:C}.");
            if (_accounts.GetByUserId(userId) is null)
                throw new AtmException("El usuario no tiene una cuenta asociada.");

            decimal total = Math.Round(amount * (1 + AtmSettings.CreditInterestRate), 2);
            var credit = new Credit
            {
                UserId = userId,
                Amount = amount,
                InterestRate = AtmSettings.CreditInterestRate,
                TotalToPay = total,
                RemainingBalance = total
            };
            _credits.Add(credit);

            if (amount <= AtmSettings.CreditAutoApprovalLimit)
                Approve(credit.Id);

            return _credits.GetById(credit.Id)!;
        }

        public void Approve(string creditId)
        {
            var credit = GetExisting(creditId);
            if (credit.Status != CreditStatus.Pending)
                throw new AtmException("Solo se pueden aprobar créditos pendientes.");

            var account = _accounts.GetByUserId(credit.UserId)
                ?? throw new AtmException("El usuario no tiene una cuenta asociada.");

            credit.Status = CreditStatus.Approved;
            credit.ApprovedAt = DateTime.UtcNow;
            _credits.Update(credit);

            _accounts.Deposit(account.Id, credit.Amount, null, TransactionType.CreditDisbursement,
                $"Desembolso de crédito {ShortId(credit.Id)}");
        }

        public void Reject(string creditId)
        {
            var credit = GetExisting(creditId);
            if (credit.Status != CreditStatus.Pending)
                throw new AtmException("Solo se pueden rechazar créditos pendientes.");

            credit.Status = CreditStatus.Rejected;
            credit.RemainingBalance = 0;
            _credits.Update(credit);
        }

        public Transaction Pay(string creditId, decimal amount, string? cardId = null)
        {
            var credit = GetExisting(creditId);
            if (credit.Status != CreditStatus.Approved || credit.RemainingBalance <= 0)
                throw new AtmException("Este crédito no tiene deuda pendiente.");
            if (amount <= 0)
                throw new AtmException("El monto debe ser mayor que cero.");
            if (amount > credit.RemainingBalance)
                throw new AtmException($"El monto excede la deuda pendiente ({credit.RemainingBalance:C}).");

            var account = _accounts.GetByUserId(credit.UserId)
                ?? throw new AtmException("El usuario no tiene una cuenta asociada.");

            // Primero se descuenta el dinero: si no hay fondos, la deuda no cambia.
            var transaction = _accounts.Withdraw(account.Id, amount, cardId, TransactionType.DebtPayment,
                $"Pago de crédito {ShortId(credit.Id)}");

            credit.RemainingBalance -= amount;
            if (credit.RemainingBalance == 0)
            {
                credit.Status = CreditStatus.Paid;
                credit.PaidAt = DateTime.UtcNow;
            }
            _credits.Update(credit);

            return transaction;
        }

        private Credit GetExisting(string creditId) =>
            _credits.GetById(creditId) ?? throw new AtmException("El crédito no existe.");

        private static string ShortId(string id) => id[..8].ToUpperInvariant();
    }
}

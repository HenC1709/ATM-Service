using ATM2.Core;
using ATM2.Core.Enums;
using ATM2.Core.Exceptions;
using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Interfaces.Services;
using ATM2.Core.Models;

namespace ATM2.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accounts;
        private readonly ITransactionService _transactions;

        public AccountService(IAccountRepository accounts, ITransactionService transactions)
        {
            _accounts = accounts;
            _transactions = transactions;
        }

        public Account? GetById(string id) => _accounts.GetById(id);

        public Account? GetByUserId(string userId) => _accounts.GetByUserId(userId);

        public Account Create(string userId, AccountType type = AccountType.Savings)
        {
            if (_accounts.GetByUserId(userId) is not null)
                throw new AtmException("El usuario ya tiene una cuenta.");

            var account = new Account { UserId = userId, Type = type };
            _accounts.Add(account);
            return account;
        }

        public decimal GetBalance(string accountId) => GetActiveAccount(accountId).Balance;

        public Transaction Deposit(string accountId, decimal amount, string? cardId = null,
            TransactionType type = TransactionType.Deposit, string? description = null)
        {
            Guard.Positive(amount);
            if (type == TransactionType.Deposit && amount > AtmSettings.MaxDepositPerTransaction)
                throw new AtmException($"El depósito máximo por operación es {AtmSettings.MaxDepositPerTransaction:C}.");

            var account = GetActiveAccount(accountId);
            account.Balance += amount;
            _accounts.Update(account);

            return Record(account, type, amount, description ?? "Depósito en cajero", cardId);
        }

        public Transaction Withdraw(string accountId, decimal amount, string? cardId = null,
            TransactionType type = TransactionType.Withdrawal, string? description = null)
        {
            Guard.Positive(amount);

            // Las reglas de billetes y límite solo aplican al retiro de efectivo, no a pagos de deuda.
            if (type == TransactionType.Withdrawal)
            {
                if (amount % AtmSettings.WithdrawalMultiple != 0)
                    throw new AtmException($"El cajero solo entrega múltiplos de {AtmSettings.WithdrawalMultiple:C}.");
                if (amount > AtmSettings.MaxWithdrawalPerTransaction)
                    throw new AtmException($"El retiro máximo por operación es {AtmSettings.MaxWithdrawalPerTransaction:C}.");
            }

            var account = GetActiveAccount(accountId);
            if (account.Balance < amount)
                throw new InsufficientFundsException(amount);

            account.Balance -= amount;
            _accounts.Update(account);

            return Record(account, type, amount, description ?? "Retiro en cajero", cardId);
        }

        public void Transfer(string sourceAccountId, string targetAccountId, decimal amount, string? cardId = null)
        {
            if (sourceAccountId == targetAccountId)
                throw new AtmException("No puedes transferirte a tu propia cuenta.");
            Guard.Positive(amount);

            var source = GetActiveAccount(sourceAccountId);
            var target = GetActiveAccount(targetAccountId);
            if (source.Balance < amount)
                throw new InsufficientFundsException(amount);

            source.Balance -= amount;
            target.Balance += amount;
            _accounts.Update(source);
            _accounts.Update(target);

            Record(source, TransactionType.TransferOut, amount, "Transferencia enviada", cardId, target.Id);
            Record(target, TransactionType.TransferIn, amount, "Transferencia recibida", null, source.Id);
        }

        private Account GetActiveAccount(string accountId)
        {
            var account = _accounts.GetById(accountId)
                ?? throw new AtmException("La cuenta no existe.");
            if (!account.IsActive)
                throw new AtmException("La cuenta está inactiva.");
            return account;
        }

        private Transaction Record(Account account, TransactionType type, decimal amount,
            string description, string? cardId, string? relatedAccountId = null)
        {
            var transaction = new Transaction
            {
                AccountId = account.Id,
                UserId = account.UserId,
                CardId = cardId,
                RelatedAccountId = relatedAccountId,
                Type = type,
                Amount = amount,
                BalanceAfter = account.Balance,
                Description = description
            };
            _transactions.Register(transaction);
            return transaction;
        }
    }
}

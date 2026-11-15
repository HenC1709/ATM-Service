using System.Security.Cryptography;
using ATM2.Core;
using ATM2.Core.Enums;
using ATM2.Core.Exceptions;
using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Interfaces.Security;
using ATM2.Core.Interfaces.Services;
using ATM2.Core.Models;

namespace ATM2.Application.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cards;
        private readonly IUserRepository _users;
        private readonly IAccountRepository _accounts;
        private readonly IPasswordHasher _hasher;

        public CardService(ICardRepository cards, IUserRepository users, IAccountRepository accounts, IPasswordHasher hasher)
        {
            _cards = cards;
            _users = users;
            _accounts = accounts;
            _hasher = hasher;
        }

        public Card Request(string userId, string pin)
        {
            var user = _users.GetById(userId) ?? throw new UserNotFoundException(userId);
            var account = _accounts.GetByUserId(userId)
                ?? throw new AtmException("El usuario no tiene una cuenta asociada.");
            ValidatePinFormat(pin);

            var card = new Card
            {
                UserId = user.Id,
                AccountId = account.Id,
                CardholderName = user.FullName.ToUpperInvariant(),
                Number = GenerateUniqueNumber(),
                PinHash = _hasher.Hash(pin),
                ExpiresAt = DateTime.UtcNow.AddYears(AtmSettings.CardValidityYears)
            };
            _cards.Add(card);
            return card;
        }

        public Card? GetById(string id) => _cards.GetById(id);

        public Card? GetByNumber(string number) => _cards.GetByNumber(number);

        public List<Card> GetByUserId(string userId) => _cards.GetByUserId(userId);

        public List<Card> GetAll() => _cards.GetAll();

        public Card ValidatePin(string cardId, string pin)
        {
            var card = GetExisting(cardId);
            EnsureUsable(card);

            if (!_hasher.Verify(pin, card.PinHash))
            {
                card.FailedPinAttempts++;
                if (card.FailedPinAttempts >= AtmSettings.MaxPinAttempts)
                {
                    card.Status = CardStatus.Blocked;
                    _cards.Update(card);
                    throw new CardBlockedException();
                }

                _cards.Update(card);
                throw new InvalidPinException(AtmSettings.MaxPinAttempts - card.FailedPinAttempts);
            }

            if (card.FailedPinAttempts > 0)
            {
                card.FailedPinAttempts = 0;
                _cards.Update(card);
            }
            return card;
        }

        public void ChangePin(string cardId, string currentPin, string newPin)
        {
            ValidatePin(cardId, currentPin);
            ValidatePinFormat(newPin);
            if (currentPin == newPin)
                throw new AtmException("El nuevo PIN debe ser diferente al actual.");

            var card = GetExisting(cardId);
            card.PinHash = _hasher.Hash(newPin);
            _cards.Update(card);
        }

        public void Block(string cardId)
        {
            var card = GetExisting(cardId);
            card.Status = CardStatus.Blocked;
            _cards.Update(card);
        }

        public void Unblock(string cardId)
        {
            var card = GetExisting(cardId);
            if (card.Status == CardStatus.Cancelled)
                throw new AtmException("Una tarjeta cancelada no se puede desbloquear.");

            card.Status = CardStatus.Active;
            card.FailedPinAttempts = 0;
            _cards.Update(card);
        }

        private Card GetExisting(string cardId) =>
            _cards.GetById(cardId) ?? throw new AtmException("La tarjeta no existe.");

        private static void EnsureUsable(Card card)
        {
            switch (card.Status)
            {
                case CardStatus.Blocked: throw new CardBlockedException();
                case CardStatus.Cancelled: throw new AtmException("La tarjeta fue cancelada.");
            }
            if (card.ExpiresAt < DateTime.UtcNow)
                throw new AtmException("La tarjeta está vencida. Solicita una nueva.");
        }

        private static void ValidatePinFormat(string pin)
        {
            if (pin.Length != AtmSettings.PinLength || !pin.All(char.IsAsciiDigit))
                throw new AtmException($"El PIN debe tener exactamente {AtmSettings.PinLength} dígitos.");
        }

        private string GenerateUniqueNumber()
        {
            string number;
            do { number = GenerateCardNumber(); }
            while (_cards.GetByNumber(number) is not null);
            return number;
        }

        /// <summary>Genera un número de 16 dígitos que empieza en 4 (tipo "Visa") y pasa el algoritmo de Luhn.</summary>
        private static string GenerateCardNumber()
        {
            int[] digits = new int[16];
            digits[0] = 4;
            for (int i = 1; i < 15; i++)
                digits[i] = RandomNumberGenerator.GetInt32(10);

            int sum = 0;
            for (int i = 0; i < 15; i++)
            {
                int d = digits[14 - i];
                if (i % 2 == 0)
                {
                    d *= 2;
                    if (d > 9) d -= 9;
                }
                sum += d;
            }
            digits[15] = (10 - sum % 10) % 10;

            return string.Concat(digits);
        }
    }
}

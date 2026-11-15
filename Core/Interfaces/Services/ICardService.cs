using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface ICardService
    {
        /// <summary>Emite una tarjeta simulada para la cuenta del usuario con el PIN indicado.</summary>
        Card Request(string userId, string pin);

        Card? GetById(string id);
        Card? GetByNumber(string number);
        List<Card> GetByUserId(string userId);
        List<Card> GetAll();

        /// <summary>
        /// Valida el PIN de la tarjeta. Tras AtmSettings.MaxPinAttempts fallos la tarjeta se bloquea.
        /// Lanza InvalidPinException o CardBlockedException.
        /// </summary>
        Card ValidatePin(string cardId, string pin);

        void ChangePin(string cardId, string currentPin, string newPin);
        void Block(string cardId);
        void Unblock(string cardId);
    }
}

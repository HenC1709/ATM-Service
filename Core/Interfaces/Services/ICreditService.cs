using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface ICreditService
    {
        Credit? GetById(string id);
        List<Credit> GetByUserId(string userId);
        List<Credit> GetPendingDebts(string userId);
        List<Credit> GetAll();

        /// <summary>
        /// Solicita un crédito. Si el monto es menor o igual a AtmSettings.CreditAutoApprovalLimit
        /// se aprueba y deposita automáticamente; si no, queda pendiente para un administrador.
        /// </summary>
        Credit Request(string userId, decimal amount);

        void Approve(string creditId);
        void Reject(string creditId);

        /// <summary>Paga (total o parcialmente) una deuda descontando de la cuenta del usuario.</summary>
        Transaction Pay(string creditId, decimal amount, string? cardId = null);
    }
}

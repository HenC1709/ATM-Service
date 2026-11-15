using ATM2.Core.Enums;
using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface ITicketService
    {
        Ticket? GetById(string id);
        List<Ticket> GetByUserId(string userId);
        List<Ticket> GetAll();
        Ticket Create(string userId, string subject, string description);
        void UpdateStatus(string ticketId, TicketStatus status);
    }
}

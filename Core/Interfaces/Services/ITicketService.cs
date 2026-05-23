using ATM2.Core.Enums;
using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface ITicketService
    {
        Ticket? GetById(string id);
        List<Ticket> GetByUserId(string userId);
        List<Ticket> GetAll();
        void Create(Ticket ticket);
        void UpdateStatus(string ticketId, TicketStatus status);
    }
}
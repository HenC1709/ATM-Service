using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Ticket? GetById(string id);
        List<Ticket> GetByUserId(string userId);
        List<Ticket> GetAll();
        void Add(Ticket ticket);
        void Update(Ticket ticket);
    }
}
using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Models;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    public class TicketRepository : JsonRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(JsonService jsonService)
            : base(jsonService, JsonContext.TicketsFile) { }

        public List<Ticket> GetByUserId(string userId) =>
            GetAll().Where(t => t.UserId == userId).ToList();
    }
}

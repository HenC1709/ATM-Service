using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Models;
using ATM2.Infrastructure.Json;

namespace ATM2.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly JsonService _jsonService;

        public TicketRepository(JsonService jsonService)
        {
            _jsonService = jsonService;
        }

        public Ticket? GetById(string id) =>
            GetAll().FirstOrDefault(t => t.Id == id);

        public List<Ticket> GetByUserId(string userId) =>
            GetAll().Where(t => t.UserId == userId).ToList();

        public List<Ticket> GetAll() =>
            _jsonService.ReadALL<Ticket>(JsonContext.TicketsFile);

        public void Add(Ticket ticket)
        {
            var tickets = GetAll();
            tickets.Add(ticket);
            _jsonService.WriteAll(JsonContext.TicketsFile, tickets);
        }

        public void Update(Ticket ticket)
        {
            var tickets = GetAll();
            int index = tickets.FindIndex(t => t.Id == ticket.Id);
            if (index != -1) tickets[index] = ticket;
            _jsonService.WriteAll(JsonContext.TicketsFile, tickets);
        }
    }
}
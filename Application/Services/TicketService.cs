using ATM2.Core.Enums;
using ATM2.Core.Exceptions;
using ATM2.Core.Interfaces.Repositories;
using ATM2.Core.Interfaces.Services;
using ATM2.Core.Models;

namespace ATM2.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _tickets;

        public TicketService(ITicketRepository tickets)
        {
            _tickets = tickets;
        }

        public Ticket? GetById(string id) => _tickets.GetById(id);

        public List<Ticket> GetByUserId(string userId) =>
            _tickets.GetByUserId(userId).OrderByDescending(t => t.CreatedAt).ToList();

        public List<Ticket> GetAll() =>
            _tickets.GetAll().OrderByDescending(t => t.CreatedAt).ToList();

        public Ticket Create(string userId, string subject, string description)
        {
            subject = subject.Trim();
            description = description.Trim();
            if (subject.Length < 3)
                throw new AtmException("El asunto debe tener al menos 3 caracteres.");
            if (description.Length < 5)
                throw new AtmException("La descripción debe tener al menos 5 caracteres.");

            var ticket = new Ticket { UserId = userId, Subject = subject, Description = description };
            _tickets.Add(ticket);
            return ticket;
        }

        public void UpdateStatus(string ticketId, TicketStatus status)
        {
            var ticket = _tickets.GetById(ticketId) ?? throw new AtmException("El ticket no existe.");
            ticket.Status = status;
            ticket.ResolvedAt = status is TicketStatus.Resolved or TicketStatus.Closed ? DateTime.UtcNow : null;
            _tickets.Update(ticket);
        }
    }
}

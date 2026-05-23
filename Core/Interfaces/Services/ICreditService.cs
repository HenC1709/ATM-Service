using ATM2.Core.Models;

namespace ATM2.Core.Interfaces.Services
{
    public interface ICreditService
    {
        Credit? GetById(string id);
        List<Credit> GetByUserId(string userId);
        List<Credit> GetAll();
        void Request(Credit credit);
        void Approve(string creditId);
        void Reject(string creditId);
    }
}
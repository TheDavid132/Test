using SanlamTest.Common.DTO;
using SanlamTest.DAL.Models;

namespace SanlamTest.DAL.Interfaces
{
    public interface IAccountRepository
    {
        Response<Account> GetById(Int64 accountId);
    }
}
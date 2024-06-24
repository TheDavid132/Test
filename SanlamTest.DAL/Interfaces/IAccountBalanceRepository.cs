using SanlamTest.Common.DTO;
using SanlamTest.DAL.Models;

namespace SanlamTest.DAL.Interfaces
{
    public interface IAccountBalanceRepository
    {
        Response<AccountBalance> GetById(Int64 accountId);
    }
}

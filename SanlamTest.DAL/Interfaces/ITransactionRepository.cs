using SanlamTest.Common.DTO;
using SanlamTest.DAL.Models;

namespace SanlamTest.DAL.Interfaces
{
    public interface ITransactionRepository
    {
        WithdrawalResult Withdraw(long accountId, decimal amount);
        Task<Response<string?>> InsertTransactionHistoryFromWithdrawOutboxAsync();
    }
}
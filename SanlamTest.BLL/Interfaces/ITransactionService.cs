using SanlamTest.BLL.DTO;

namespace SanlamTest.BLL.Interfaces
{
    public interface ITransactionService
    {
        WithdrawalResponseDto Withdraw(long accountId, decimal amount);
        Task InsertTransactionHistoryFromWithdrawOutboxAsync(int concurrencyCount);
    }
}
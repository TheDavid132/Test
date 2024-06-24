namespace SanlamTest.BLL.Interfaces
{
    public interface ITransactionMessageService
    {
        Task SendWithdrawalMessages(int concurrentMessages, int retryAttempts, CancellationToken cancellationToken);
    }
}
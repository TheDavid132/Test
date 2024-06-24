using SanlamTest.DAL.Models;

namespace SanlamTest.DAL.Interfaces
{
    public interface IMessageHandler
    {
        Task PublishWithdrawalEventAsync(WithdrawalEvent withdrawalEvent);
    }
}
using SanlamTest.Common.DTO;
using SanlamTest.Common.Enums;
using SanlamTest.DAL.Models;

namespace SanlamTest.DAL.Interfaces
{
    public interface IWithdrawOutboxRepository
    {
        Response<IEnumerable<WithdrawOutbox>> GetPendingMessagesSetStatusInProgress(int count);
        Status ResetMessageServiceStatus(int WithdrawOutboxId);
    }
}
using System.Text.Json;

namespace SanlamTest.BLL.DTO
{
    public class WithdrawalEventDto
    {
        public decimal Amount { get; set; }
        public long AccountId { get; set; }
        public string Status { get; set; } // Example: "SUCCESSFUL", "FAILED", etc.

        public WithdrawalEventDto(decimal amount, long accountId, string status)
        {
            Amount = amount;
            AccountId = accountId;
            Status = status;
        }
    }
}

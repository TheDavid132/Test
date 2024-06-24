using System.Text.Json;

namespace SanlamTest.DAL.Models
{
    public class WithdrawalEvent
    {
        public decimal Amount { get; set; }
        public Int64 AccountId { get; set; }
        public string Status { get; set; } // Example: "SUCCESSFUL", "FAILED", etc.

        public WithdrawalEvent(decimal amount, Int64 accountId, string status)
        {
            Amount = amount;
            AccountId = accountId;
            Status = status;
        }
    }
}

using SanlamTest.Common.Enums;

namespace SanlamTest.DAL.Models
{
    public class WithdrawalResult
    {
        public WithdrawalStatus Status { get; set; }
        public decimal Balance { get; set; }
    }
}
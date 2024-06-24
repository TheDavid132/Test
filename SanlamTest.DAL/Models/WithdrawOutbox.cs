namespace SanlamTest.DAL.Models
{
    public class WithdrawOutbox
    {
        public int Id { get; set; }
        public Guid TransactionGuid { get; set; }
        public Int64 AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal RunningBalance { get; set; }
        public int TransactionHistoryStatusId { get; set; }
        public int MessageServiceStatusId { get; set; }
        public DateTime TransactionHistoryStatusChangeDate { get; set; }
        public DateTime MessageServiceStatusChangeDate { get; set; }
    }

}

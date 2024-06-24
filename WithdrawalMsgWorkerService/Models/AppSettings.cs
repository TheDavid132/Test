namespace WithdrawalMsgWorkerService.Models
{
    public class AppSettings
    {
        public int ConcurrencyCount { get; set; }
        public int ServiceSleepTimeInMs { get; set; }

        public int RetryIntervalInSeconds { get; set; }
        public int RetryAttempts { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
    }

    public class Connectionstrings
    {
        public string SanlamDb { get; set; }
    }

}
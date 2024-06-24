namespace TransactioHistoryWorkerService.Models
{
    public class AppSettings
    {       
        public int ConcurrencyCount { get; set; }
        public int SleepTimeInMs { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }     
    }

    public class Connectionstrings
    {
        public string SanlamDb { get; set; }
    }

}
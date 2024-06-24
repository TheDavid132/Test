namespace SanlamTest.Common.Interfaces
{
    public interface IRetryHelper
    {
        Task RetryAsync(Func<Task> action, int retryCount, Func<Task> fallback);
    }
}
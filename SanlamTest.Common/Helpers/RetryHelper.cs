namespace SanlamTest.Common.Helpers
{
    public class OperationResult
    {
        public bool Success { get; set; }        
        public string ErrorMessage { get; set; }

        public OperationResult(bool success, string errorMessage = null)
        {
            Success = success;            
            ErrorMessage = errorMessage;
        }
    }

    public class RetryHelper
    {
        public static async Task<OperationResult> PerformOperationWithRetryAsync(
        Func<Task<OperationResult>> operation,
        int maxRetries,
        TimeSpan delay,
        CancellationToken cancellationToken,
        Func<Task<OperationResult>> fallbackOperation)
        {
            int retryCount = 0;

            while (retryCount <= maxRetries)
            {
                try
                {
                    // Check for cancellation before each retry attempt
                    cancellationToken.ThrowIfCancellationRequested();

                    // Perform the operation
                    var result = await operation();

                    // If operation is successful, return the result
                    if (result.Success)
                        return result;

                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation
                    
                    return await fallbackOperation();
                }
                catch (Exception ex)
                {
                    // Log or handle other exceptions
                    // Increment the retry count
                    retryCount++;
                    return new OperationResult(false, ex.Message);
                }
            }

            // If all retry attempts fail, try fallback operation
            return await fallbackOperation();
        }
    }
}

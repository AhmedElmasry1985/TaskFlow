namespace Core.Helpers;

public static class Retry
{
    /// <summary>
    /// Executes a synchronous function with retry logic
    /// </summary>
    /// <typeparam name="TResult">The return type of the function</typeparam>
    /// <param name="func">The function to execute</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3)</param>
    /// <param name="delayMilliseconds">Delay between retries in milliseconds (default: 1000)</param>
    /// <param name="onRetry">Optional callback invoked on each retry with attempt number and exception</param>
    /// <returns>The result of the function</returns>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail</exception>
    public static TResult Execute<TResult>(
        Func<TResult> func,
        int maxRetries = 3,
        int delayMilliseconds = 1000,
        Action<int, Exception>? onRetry = null)
    {
        var exceptions = new List<Exception>();
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
                
                if (attempt < maxRetries)
                {
                    onRetry?.Invoke(attempt, ex);
                    Thread.Sleep(delayMilliseconds);
                }
            }
        }
        
        throw new AggregateException(
            $"Operation failed after {maxRetries} attempts",
            exceptions);
    }

    /// <summary>
    /// Executes a synchronous action with retry logic
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3)</param>
    /// <param name="delayMilliseconds">Delay between retries in milliseconds (default: 1000)</param>
    /// <param name="onRetry">Optional callback invoked on each retry with attempt number and exception</param>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail</exception>
    public static void Execute(
        Action action,
        int maxRetries = 3,
        int delayMilliseconds = 1000,
        Action<int, Exception>? onRetry = null)
    {
        Execute(() =>
        {
            action();
            return true;
        }, maxRetries, delayMilliseconds, onRetry);
    }

    /// <summary>
    /// Executes an asynchronous function with retry logic
    /// </summary>
    /// <typeparam name="TResult">The return type of the function</typeparam>
    /// <param name="func">The async function to execute</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3)</param>
    /// <param name="delayMilliseconds">Delay between retries in milliseconds (default: 1000)</param>
    /// <param name="onRetry">Optional callback invoked on each retry with attempt number and exception</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
    /// <returns>The result of the function</returns>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail</exception>
    public static async Task<TResult> ExecuteAsync<TResult>(
        Func<Task<TResult>> func,
        int maxRetries = 3,
        int delayMilliseconds = 1000,
        Action<int, Exception>? onRetry = null,
        CancellationToken cancellationToken = default)
    {
        var exceptions = new List<Exception>();
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
                
                if (attempt < maxRetries)
                {
                    onRetry?.Invoke(attempt, ex);
                    await Task.Delay(delayMilliseconds, cancellationToken);
                }
            }
        }
        
        throw new AggregateException(
            $"Operation failed after {maxRetries} attempts",
            exceptions);
    }

    /// <summary>
    /// Executes an asynchronous action with retry logic
    /// </summary>
    /// <param name="action">The async action to execute</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3)</param>
    /// <param name="delayMilliseconds">Delay between retries in milliseconds (default: 1000)</param>
    /// <param name="onRetry">Optional callback invoked on each retry with attempt number and exception</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail</exception>
    public static async Task ExecuteAsync(
        Func<Task> action,
        int maxRetries = 3,
        int delayMilliseconds = 1000,
        Action<int, Exception>? onRetry = null,
        CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(async () =>
        {
            await action();
            return true;
        }, maxRetries, delayMilliseconds, onRetry, cancellationToken);
    }

    /// <summary>
    /// Executes an asynchronous function with exponential backoff retry logic
    /// </summary>
    /// <typeparam name="TResult">The return type of the function</typeparam>
    /// <param name="func">The async function to execute</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3)</param>
    /// <param name="initialDelayMilliseconds">Initial delay in milliseconds (default: 1000)</param>
    /// <param name="maxDelayMilliseconds">Maximum delay in milliseconds (default: 30000)</param>
    /// <param name="onRetry">Optional callback invoked on each retry with attempt number and exception</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
    /// <returns>The result of the function</returns>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail</exception>
    public static async Task<TResult> ExecuteWithExponentialBackoffAsync<TResult>(
        Func<Task<TResult>> func,
        int maxRetries = 3,
        int initialDelayMilliseconds = 1000,
        int maxDelayMilliseconds = 30000,
        Action<int, Exception>? onRetry = null,
        CancellationToken cancellationToken = default)
    {
        var exceptions = new List<Exception>();
        int delay = initialDelayMilliseconds;
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
                
                if (attempt < maxRetries)
                {
                    onRetry?.Invoke(attempt, ex);
                    await Task.Delay(delay, cancellationToken);
                    
                    // Exponential backoff: double the delay, but cap at maxDelay
                    delay = Math.Min(delay * 2, maxDelayMilliseconds);
                }
            }
        }
        
        throw new AggregateException(
            $"Operation failed after {maxRetries} attempts with exponential backoff",
            exceptions);
    }
}
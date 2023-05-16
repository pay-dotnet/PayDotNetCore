namespace PayDotNet.Core.Managers;

internal static class Synchronizer
{
    internal static async Task<TResult> Retry<TResult>(Func<Task<TResult>> task, int retries = 1)
    {
        for (int @try = 0; @try <= retries; @try++)
        {
            try
            {
                return await task();
            }
            catch (Exception)
            {
                if (@try <= retries)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }
                else
                {
                    throw;
                }
            }
        }

        throw new PayDotNetException("Unreachable code in retry mechanism.");
    }
}
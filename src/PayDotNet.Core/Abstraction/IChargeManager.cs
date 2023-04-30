namespace PayDotNet.Core.Abstraction;

public interface IChargeManager
{
    Task SynchroniseAsync(string processor, string processorId, string customerProcessorId, int attempt = 0, int retries = 1);
}
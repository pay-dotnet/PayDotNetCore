using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IChargeManager
{
    Task<PayCharge> SynchroniseAsync(string chargeId, int attempt = 0, int retries = 1);

    Task<PayCharge> SynchroniseAsync(PayCharge payCharge, int attempt = 0, int retries = 1);
}
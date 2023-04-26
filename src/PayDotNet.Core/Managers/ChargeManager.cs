using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Managers;

public class ChargeManager : IChargeManager
{
    public Task<PayCharge> SynchroniseAsync(string chargeId, int attempt = 0, int retries = 1)
    {
        return Task.FromResult<PayCharge>(new());
    }

    public Task<PayCharge> SynchroniseAsync(PayCharge payCharge, int attempt = 0, int retries = 1)
    {
        return Task.FromResult<PayCharge>(new());
    }
}
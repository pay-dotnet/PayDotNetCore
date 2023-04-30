using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Managers;

public class MerchantManager : IMerchantManager
{
    public Task<PayMerchant?> FindByIdAsync(string processorName, string processorId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(PayMerchant payMerchant)
    {
        throw new NotImplementedException();
    }
}
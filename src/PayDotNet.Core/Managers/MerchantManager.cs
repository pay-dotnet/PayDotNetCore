using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Managers;

public class MerchantManager : IMerchantManager
{
    public virtual Task<PayMerchant?> FindByIdAsync(string processorName, string processorId)
    {
        throw new NotImplementedException();
    }

    public virtual Task UpdateAsync(PayMerchant payMerchant)
    {
        throw new NotImplementedException();
    }
}
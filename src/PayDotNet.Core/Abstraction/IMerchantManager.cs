using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IMerchantManager
{
    Task<PayMerchant?> FindByIdAsync(string processorName, string processorId);

    Task UpdateAsync(PayMerchant payMerchant);
}
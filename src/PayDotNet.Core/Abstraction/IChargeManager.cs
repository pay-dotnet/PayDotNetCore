using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IChargeManager
{
    Task SynchroniseAsync(string processor, string processorId, string customerProcessorId, int attempt = 0, int retries = 1);

    Task<PayCharge?> GetAsync(string processorId);

    Task<IPayment> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options);

    Task<IPayment> CaptureAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeOptions options);

    Task RefundAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options);

    Task<ICollection<PayChargeRefund>> GetCreditNotesAsync(PayCharge payCharge);
}
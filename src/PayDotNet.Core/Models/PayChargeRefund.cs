namespace PayDotNet.Core.Models;

public class PayChargeRefund
{
    public string ProcessorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public int Amount { get; set; }
    public string Reason { get; set; }
    public string Status { get; set; }
}

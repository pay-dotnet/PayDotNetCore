using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

/// <summary>
/// A facade interface containing the primary operations for Pay
/// </summary>
public interface IBillableManager
{
    /// <summary>
    /// Retrieves or create a new customer based on processorName and email.
    /// </summary>
    /// <param name="email">The email of the customer</param>
    /// <param name="options">The options for the customer.</param>
    /// <returns>The pay customer that is used in the other operations.</returns>
    Task<PayCustomer> GetOrCreateCustomerAsync(string email, PayCustomerOptions options);

    /// <summary>
    /// Creates a subscription for the customer.
    /// </summary>
    /// <param name="payCustomer">The pay customer. MUST HAVE A PROCESSOR ID</param>
    /// <param name="options">The subscription options</param>
    /// <returns>returns a payment that can be used to determine if everything was successful.</returns>
    Task<IPayment> SubscribeAsync(PayCustomer payCustomer, PaySubscribeOptions options);

    /// <summary>
    /// Charges the customer one-time amount depending on the options.
    /// </summary>
    /// <param name="payCustomer"></param>
    /// <param name="options">The options for the charge.</param>
    /// <returns>returns a payment that can be used to determine if everything was successful.</returns>
    Task<IPayment> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options);

    /// <summary>
    /// Starts a checkout session in the payment processor.
    /// This method returns an URL that can be used to redirect the user to.
    ///
    /// Usage examples for options:
    /// <code>
    /// new PayCheckoutOptions(mode: "payment");
    /// new PayCheckoutOptions(mode: "setup");
    /// new PayCheckoutOptions(mode: "subscription");
    ///
    /// new PayCheckoutOptions(lineItem: "price_12345", quantity: 2);
    /// new PayCheckoutOptions(lineItems: new[] { "price_123", "price_456" });
    /// new PayCheckoutOptions(lineItem: "price_12345", allowPromotionCodes: true);
    /// </code>
    /// </summary>
    /// <param name="payCustomer">The pay customer. MUST HAVE A PROCESSOR ID</param>
    /// <param name="options">The options for the checkout.</param>
    /// <returns>The checkout result with an URI and payment data..</returns>
    Task<PayCheckoutResult> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options);

    /// <summary>
    /// Starts a checkout session in the payment processor based on a specified shopping card.
    /// The options parameter can either contain a simple or complex shopping cart.
    ///
    /// Usage examples for options
    /// <code>
    /// new PayCheckoutChargeOptions(name: "T-Shirt", unitAmount: 15_00, currency: "eur", quantity: 2);
    ///
    /// new PayCheckoutChargeOptions(products: new[] {
    ///     new PayCheckoutLineItemProductData(Name: "Appels", UnitAmount: 0_99, Currency: "eur", new()),
    ///     new PayCheckoutLineItemProductData(Name: "Pears", UnitAmount: 0_75, Currency: "eur", new())
    /// });
    ///
    /// new PayCheckoutChargeOptions(lineItems: new[] {
    ///     new PayCheckoutLineItem(null, PriceData: new(Name: "Appels", UnitAmount: 0_99, Currency: "eur", new()), Quantity: 2),
    ///     new PayCheckoutLineItem(null, PriceData: new(Name: "Pears", UnitAmount: 0_75, Currency: "eur", new()), Quantity: 10)
    /// });
    /// </code>
    ///
    /// The underlying implementation just uses <see cref="CheckoutAsync(PayCustomer, PayCheckoutOptions)"/>.
    /// </summary>
    /// <param name="payCustomer">The pay customer. MUST HAVE A PROCESSOR ID</param>
    /// <param name="options">The options for the checkout charge.</param>
    /// <returns>The checkout result with an URI and payment data..</returns>
    Task<PayCheckoutResult> CheckoutChargeAsync(PayCustomer payCustomer, PayCheckoutChargeOptions options);
}
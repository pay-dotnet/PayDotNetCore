using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Api;

public class StripeWebhookController : Controller
{
    private readonly ILogger<StripeWebhookController> _logger;
    private readonly IOptions<PayDotNetConfiguration> _options;
    private readonly IWebhookManager _webhookManager;

    public StripeWebhookController(
        IWebhookManager webhookManager,
        ILogger<StripeWebhookController> logger,
        IOptions<PayDotNetConfiguration> options)
    {
        _webhookManager = webhookManager;
        _logger = logger;
        _options = options;
    }

    [HttpPost]
    [Route("pay/webhooks/stripe")]
    public async Task<IActionResult> HandleWebhookAsync()
    {
        string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            Event stripeEvent = await GetVerifiedEvent(json);
            PayWebhook payWebhook = new(stripeEvent.Id, PaymentProcessors.Stripe, stripeEvent.Type, json, stripeEvent.Created);
            await _webhookManager.HandleAsync(payWebhook);
            return Ok();
        }
        catch (StripeException stripeException)
        {
            _logger.LogError(stripeException, "StripeException occured");
            return BadRequest();
        }
    }

    private Task<Event> GetVerifiedEvent(string json)
    {
        if (string.IsNullOrEmpty(_options.Value.Stripe.EndpointSecret))
        {
            throw new StripeException("Cannot verify signature without a Stripe signing secret");
        }

        string? signature = HttpContext.Request.Headers["Stripe-Signature"];
        if (string.IsNullOrEmpty(_options.Value.Stripe.EndpointSecret))
        {
            throw new StripeException("Cannot verify signature without the 'Stripe-Signature' header");
        }

        Event stripeEvent = EventUtility.ConstructEvent(json, signature, _options.Value.Stripe.EndpointSecret);
        return Task.FromResult(stripeEvent);
    }
}
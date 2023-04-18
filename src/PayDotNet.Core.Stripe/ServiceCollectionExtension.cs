using PayDotNet.Core.Services;
using PayDotNet.Core.Stripe;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static PayDotNetBuilder AddStripe(this PayDotNetBuilder builder)
    {
        builder.Services.AddSingleton<IPaymentProcessorService, StripePaymentProcessorService>();
        return builder;
    }
}
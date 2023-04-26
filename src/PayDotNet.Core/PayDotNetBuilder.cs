using Microsoft.Extensions.DependencyInjection;

namespace PayDotNet.Core;

/// <summary>
/// Builder class for other packages to register their own services.
/// </summary>
public sealed class PayDotNetBuilder
{
    public IServiceCollection Services { get; }

    public PayDotNetBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
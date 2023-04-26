using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PayDotNet.Core;

/// <summary>
/// Builder class for other packages to register their own services.
/// </summary>
public sealed class PayDotNetBuilder
{
    public IServiceCollection Services { get; }

    public IConfiguration Configuration { get; }

    public PayDotNetConfiguration PayDotNetConfiguration { get; }

    public PayDotNetBuilder(IServiceCollection services, IConfiguration configuration, PayDotNetConfiguration payDotNetConfiguration)
    {
        Services = services;
        Configuration = configuration;
        PayDotNetConfiguration = payDotNetConfiguration;
    }
}
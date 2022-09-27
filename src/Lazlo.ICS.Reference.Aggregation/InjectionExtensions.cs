using Microsoft.Extensions.DependencyInjection;
using ThreeTwoSix.SDK.Messaging.Abstractions;

namespace Lazlo.ICS.Reference.Aggregation;

public static class InjectionExtensions
{
    public static void AddAggregationHandler(
        this IServiceCollection services,
        string privateKeyPEM)
    {
        services.AddSingleton<IEventHandler, AggregationHandler>();

        services
            .AddOptions<AggregationHandlerOptions>()
            .Configure((options) =>
            {
                if (string.IsNullOrWhiteSpace(privateKeyPEM))
                {
                    throw new ArgumentException($"PrivateKeyPEM cannot be null or whitespace.");
                }

                options.PrivateKeyPEM = privateKeyPEM;
            });
    }
}

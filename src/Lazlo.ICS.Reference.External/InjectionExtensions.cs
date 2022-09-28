using ThreeTwoSix.SDK.Messaging.Abstractions;
using ThreeTwoSix.SDK.Messaging.Sinks;

namespace Microsoft.Extensions.DependencyInjection;

public static class InjectionExtensions
{
    public static void AddFileSink(
    this IServiceCollection services,
    string outputBasePath)
    {
        services.AddSingleton<IEventSink, FileSinkRepository>();

        services
            .AddOptions<FileSinkOptions>()
            .Configure((options) =>
            {
                if (string.IsNullOrWhiteSpace(outputBasePath))
                {
                    throw new ArgumentException($"OutputBasePath cannot be null or whitespace.");
                }

                options.OutputBasePath = outputBasePath;
            });
    }
}

using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ThreeTwoSix.SDK.Messaging.Abstractions;

namespace ThreeTwoSix.SDK.Messaging.Sinks;

public class FileSinkRepository : IEventSink
{
    private StreamWriter OutputStream { get; }

    private JsonSerializer JsonSerializer { get; } = new JsonSerializer();

    public FileSinkRepository(
        IOptions<FileSinkOptions> options)
    {
        if (string.IsNullOrWhiteSpace(options.Value.OutputBasePath))
        {
            throw new ArgumentException("OutputBashPath option cannot be null or whitespace");
        }

        if (!Directory.Exists($@"{options.Value.OutputBasePath}"))
        {
            Directory.CreateDirectory($@"{options.Value.OutputBasePath}");
        }

        OutputStream = File.CreateText($@"{options.Value.OutputBasePath}\{Guid.NewGuid()}.json");
    }

    public Task AddEventAsync<T>(T icsEvent)
    {
        // This is quick and dirty
        // You should write individual events OR discriminate by partition as the lock could cause latency
        lock (OutputStream)
        {
            JsonSerializer.Serialize(OutputStream, icsEvent);
        }

        return Task.CompletedTask;
    }
}

using Azure.Messaging.EventHubs.Processor;

namespace ThreeTwoSix.SDK.Messaging.Abstractions;

public interface IEventHandler
{
    Task ProcessEventAsync(ProcessEventArgs eventArgs);

    Task ProcessErrorAsync(ProcessErrorEventArgs eventArgs);
}

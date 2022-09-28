namespace ThreeTwoSix.SDK.Messaging.Abstractions;

public interface IEventSink
{
    Task AddEventAsync<T>(T sinkEvent);
}
using Azure.Messaging.EventHubs;
using Microsoft.ApplicationInsights;
using ThreeTwoSix.SDK.Messaging.Abstractions;

namespace Lazlo.ICS.Reference.Worker
{
    public class ICSBackgroundService : BackgroundService
    {
        private EventProcessorClient EventProcessor { get; }

        private ILogger<ICSBackgroundService> Logger { get; }

        private TelemetryClient TelemetryClient { get; }

        public ICSBackgroundService(
            ILogger<ICSBackgroundService> logger,
            TelemetryClient telemetryClient,
            EventProcessorClient eventProcessor,
            IEventHandler eventHandler)
        {
            EventProcessor = eventProcessor;

            Logger = logger;

            TelemetryClient = telemetryClient;

            EventProcessor.ProcessEventAsync += eventHandler.ProcessEventAsync;
            EventProcessor.ProcessErrorAsync += eventHandler.ProcessErrorAsync;
        }

        private async Task TickLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while(true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

                    Logger.LogInformation($"Tick");
                }
            }

            catch (TaskCanceledException)
            {

            }            
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await EventProcessor.StartProcessingAsync(cancellationToken).ConfigureAwait(false);

                await TickLoopAsync(cancellationToken).ConfigureAwait(false);

                await EventProcessor.StopProcessingAsync().ConfigureAwait(false);
            }

            catch (Exception ex)
            {
                Logger.Log(LogLevel.Critical, ex, null);

                TelemetryClient.TrackException(ex);
            }
        }
    }
}
using Azure.Messaging.EventHubs.Processor;
using Lazlo.Gaming.SDK.Common;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Security.Cryptography;
using ThreeTwoSix.SDK.Cryptography.Helpers;
using ThreeTwoSix.SDK.DataStream;
using ThreeTwoSix.SDK.Messaging.Abstractions;

namespace Lazlo.ICS.Reference.Aggregation;

public class AggregationHandler : IEventHandler
{
    private IEventSink EventSink { get; }

    private ILogger<AggregationHandler> Logger { get; }

    private TelemetryClient TelemetryClient { get; }

    private RSAParameters RSAParameters { get; }

    public AggregationHandler(
        ILogger<AggregationHandler> logger,
        TelemetryClient telemetryClient,
        IEventSink eventSink,
        IOptions<AggregationHandlerOptions> options)
    {
        EventSink = eventSink;
        Logger = logger;
        TelemetryClient = telemetryClient;

        RSAParameters = PemHelper.ParseRsaParameters(options.Value.PrivateKeyPEM);
    }

    public Task ProcessErrorAsync(ProcessErrorEventArgs eventArgs)
    {
        //TODO report detailed data about events

        string message = $"\t*** Partition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen. {eventArgs.Exception.Message}";

        // Write details about the error to the console window
        Logger.LogError(eventArgs.Exception, message);

        TelemetryClient.TrackException(eventArgs.Exception);

        return Task.CompletedTask;
    }

    public async Task ProcessEventAsync(ProcessEventArgs eventArgs)
    {
        if (!eventArgs.HasEvent)
        {
            var msg = $"{eventArgs.Partition} Empty Event";

            Logger.LogInformation(msg);

            TelemetryClient.TrackEvent(msg);
        }

        try
        {
            // Events CAN arrive empty from cloud provider resources, not the same as missing ICS data
            if (eventArgs.Data != null && eventArgs.Data.EventBody != null)
            {
                // If this is a direct ICS EventItem
                if (eventArgs.Data.Properties.Any(p => p.Key == "EventType"))
                {
                    // all ICS events arrive as DataStreamEvent types available in the ThreeTwoSix.SDK.DataStream Nuget package
                    DataStreamEvent se = JsonConvert.DeserializeObject<DataStreamEvent>(eventArgs.Data.EventBody.ToString());

                    using RSA rsa = RSA.Create(RSAParameters);

                    // Decrypt the Symmetric Data Encryption "Key" which was encrypted using Asymmetric (RSA)
                    byte[] dekKey = rsa.Decrypt(se.CypherBytesSecure.DekKeyCypherBytes, RSAEncryptionPadding.OaepSHA256);

                    using Aes aes = Aes.Create();

                    ICryptoTransform decryptor = aes.CreateDecryptor(dekKey, se.CypherBytesSecure.DekIV);
                    
                    using MemoryStream msDecrypt = new(se.CypherBytesSecure.CypherBytes);
                    using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                    using BrotliStream ingflator = new(csDecrypt, CompressionMode.Decompress, false);

                    using MemoryStream inflatedStream = new();
                    await ingflator.CopyToAsync(inflatedStream);

                    string json = Encoding.UTF8.GetString(inflatedStream.ToArray());

                    string eventType = (string)eventArgs.Data.Properties.First(p => p.Key == "EventType").Value;

                    switch (eventType)
                    {
                        case var i when i.Contains("Panel"):

                            await ProcessPanelAsync(json);

                            break;

                        case var i when i.Contains("DrawPrizesCalculated"):

                            break;

                        default:

                            break;
                    }
                }

                // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
                await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
            }
        }

        catch (Exception ex)
        {
            Logger.LogInformation($"*** {ex.Message}");

            TelemetryClient.TrackEvent($"*** {ex.Message}");
        }

        finally
        {
            //_logger.LogInformation($"{nameof(ProcessEventAsync)} End");
        }
    }

    private async Task ProcessPanelAsync(string json)
    {
        try
        {
            FinalizedPanel panel = JsonConvert.DeserializeObject<FinalizedPanel>(json);

            await EventSink.AddEventAsync(panel);
        }

        catch (Exception ex)
        {
            Logger.Log(LogLevel.Critical, ex, null);

            TelemetryClient.TrackException(ex);

            throw;
        }
    }
}
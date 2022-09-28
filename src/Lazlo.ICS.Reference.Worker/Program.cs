using Azure.Messaging.EventHubs;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Diagnostics;
using ThreeTwoSix.SDK.Messaging.Sinks;
using Lazlo.ICS.Reference.Aggregation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Lazlo.ICS.Reference.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(config =>
        {
            if (File.Exists($"appsettings.json"))
            {
                config.AddJsonFile($"appsettings.json", optional: true);
            }

            string myEnv = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            if (myEnv != null && File.Exists($"appsettings.{myEnv}.json"))
            {
                config.AddJsonFile($"appsettings.{myEnv}.json", optional: true);
            }

            var settings = config.Build();
        })
        .ConfigureServices((hostContext, services) =>
        {
            IConfiguration configuration = hostContext.Configuration;

            if (configuration != null)
            {
                var aiOptions = new ApplicationInsightsServiceOptions
                {
                    ConnectionString = ""
                };

                services.AddApplicationInsightsTelemetryWorkerService(aiOptions);

                services.AddSingleton(
                    new EventProcessorClient(
                        new BlobContainerClient(
                            "DefaultEndpointsProtocol=https;AccountName=ldvdevorchacme;AccountKey=Rkp42KoBQ45YO70dE3/EjKOLstWjemR6nVQBkGM6UdOCO5VkscnIKKWEyfaiYnemfJkJ+M4pxkwx+AStkw8hfw==;EndpointSuffix=core.windows.net",
                            // You need a storage account container to store the checkpoints of the multiple clients reading the stream
                            "orchestration",
                            new BlobClientOptions()
                            {
                                //TODO shoud config this...
                                //GeoRedundantSecondaryUri = ""
                            }
                        ),
                        // TIP: use alternate consumer groups to allow multiple developers to see different segments of the stream using different checkpoints
                        "jack.bond",
                        "Endpoint=sb://ldv-dev-integration-acme.servicebus.windows.net/;SharedAccessKeyName=listenonlyrule;SharedAccessKey=FPe+iVjo4578AmFug1vaPt3jrHLvZ1GLTOU3S1EAjrU=;EntityPath=topiclotteryevents",
                        new EventProcessorClientOptions()
                        {
                            PartitionOwnershipExpirationInterval = TimeSpan.FromMinutes(5),
                            //TODO should name this...
                            //Identifier = 
                        }
                        )
                    );
            }

            string privateKey = @"-----BEGIN RSA PRIVATE KEY-----
                    MIIEpAIBAAKCAQEA433F1om5gyRBoCFT/8y4Kp63QLsKJQ2EqSX2Llphp9HqNsJ9
                    aDeZOi5JRuX1ZX7341gJyAbH7cyc1KeMYyalxIsXQ91cw0WC0C40nGNYQgZG3igw
                    0BOokEMnclbIFUIiY9xYTio7djh7vqhV+oU2pVKpAV2PAMAi+NOZ+MqheIxs+mGy
                    E5p2J4xn3i7/06VEZ6aiAkPudX4gQ5p4xxm5cLyWn8Dx6N97PnaJ3fa+O2HigxCT
                    YD4GvMVKYKMgkOjOD52cm5L8BMHbONcdpOd5g6GlLQMD8N9EGcEU1Jw1HV/bHD4u
                    QG0uAUz7jzGJz4dHY73K3JUZGyg79P8Q6Tw8qQIDAQABAoIBAQC2Qv9tqnxqGHNH
                    s0wUZtWqt+zEPNac5x7BUnvRmXIiPalz5BELnXfzSEBHQFiCz94VVGTJ0Lz/xe5k
                    5jQxADbRqEqkgccfYCK1Xj+iiGmexF6lAPhSzV1A77y7+9FkTs80yMYrIeTwC8MA
                    9uejxnUIsZhcUrQO+uFKEGaNEPObmws8XwkvYUSs+E5qrMI/QfWL8Af76RJFbU/7
                    p4g7MlYZwp0epD2tDp4IO1NcCE0SHReKo8rsB45tyld1oX2HhaApZPl5/dls3ZhP
                    rSkwXB/yS1WFY7p11qO7ddwibGOriyYLwYhjgQkyP2dLcup183ywBGYr+hGh7QDf
                    /B2lm6htAoGBAP5toyeM4zIslO/aS9OvB3l4dyhCCUztYiR4hzMXgqSNj2GD5bme
                    vNX1ckwG9LupoUS6jzl6JY+N0RH/BcG67wLLjkQ43xURaItRo2v+J0C/GNCzvpm0
                    jYtVrxYoOLxgC9Jf308e36W2EhaDmr/V2RzKAarzMfsADUdymiLaI2H7AoGBAOTl
                    iUwpQmJKWG6CCoh2GGxarbVmBsEc7W4OfKNYqpATxIK8ZXgapWl+CUyrZdYSuWq2
                    l0YTYxLfUqnB268XkM4/5UPdVKvKy0WrBVVFn6qQzvrrLtsnvnQI8HU8Q2FgVUUG
                    9+oDWgiVI+xjeDhCvFYkOqJemo8ssDJF01GebT6rAoGAUJJDwriZEkCQAcztimG5
                    Sjxd77/J1jSuicIpfoKJerbhmw375+ZfApqx0WW6httXGL7DsH3/+w/8D2jlV85s
                    9kOkD/K6op9arhPyXrajk5twrlbdmytUT7WYtrmSDgWUeNCnlRS/2mhoHf0bOnjs
                    QnOuR4awYz5G9kNSkIrn/ZUCgYBP/BmnIH/PAvW14AE4QDQ2oNU0nytbDfW10KAj
                    IFexswanPJgkiQMmQuGTBg226aIbNSTVWu7y6FDlexV+MLjsKY2+0jfFND8l4CYj
                    7wllO+bn7YjZEiFOQNVt0holi9kgHthA0N0ERMFh2DxpRIC1hUFr/az6vP3xA09d
                    pQO9swKBgQCK81eqNz5pdyiK7vFlWBJs1ir6bfJFgxdj8iL/Ze+Rh2msNZG6eYZR
                    yAeg3GXgCaL6AFHDeJo+ZwAtenG8hQKKKXKRvCyGndKpjehMeFQABctNApJf5J7U
                    qsQjAH/tUOsVx8eBme6F6vvZf8JDqc5sHkIEy0taltHYy2eA9Huj2w==
                    -----END RSA PRIVATE KEY-----";

            services.AddAggregationHandler(privateKey);

            // Incoming events will be written to a random file in this directory
            services.AddFileSink(@"C:\ICS");

            // The hosted service executes the micro service
            services.AddHostedService<ICSBackgroundService>();
        });
}
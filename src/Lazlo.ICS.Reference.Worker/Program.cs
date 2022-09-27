using Azure.Messaging.EventHubs;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Diagnostics;
using ThreeTwoSix.SDK.Messaging.Sinks;
using Lazlo.ICS.Reference.Aggregation;

namespace Lazlo.ICS.Reference.Worker
{
    public class Program
    {
        private static IConfiguration? _configuration = null;
        private static IConfigurationRefresher? _refresher = null;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                if (File.Exists($"appsettings.json")) config.AddJsonFile($"appsettings.json", optional: true);

                var myEnv = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

                if (myEnv != null && File.Exists($"appsettings.{myEnv}.json")) config.AddJsonFile($"appsettings.{myEnv}.json", optional: true);
                else Debugger.Break();

                var settings = config.Build();

                //#region Credential

                // You can create an app registration to drive RBAC for this app
                // connnecting to services can be via app registration or managed identity, your choice
                // if deploying as a container, remember to pass these as environment variables to the container!

                //ClientSecretCredential? credential = null;

                //if (myEnv != null)
                //{
                //    var tenantID = settings.GetValue<string>("ADCredential:tenantID");
                //    var clientID = settings.GetValue<string>("ADCredential:clientID");
                //    var clientSecret = settings.GetValue<string>("ADCredential:clientSecret");

                //    credential = new ClientSecretCredential(
                //        tenantID,
                //        clientID,
                //        clientSecret
                //    );
                //}

                //#endregion Credential

                ////var acPath = Environment.GetEnvironmentVariable("azureappconfig_operations_uri_ncus");
                //var acCS = Environment.GetEnvironmentVariable("azureappconfig_operations_connectionstring_ncus");

                //if (acCS != null)
                //{
                //    config.AddAzureAppConfiguration(options =>
                //    {
                //        options.Connect(acCS)
                //                .ConfigureKeyVault(kv =>
                //                {
                //                    kv.SetCredential(
                //                        credential
                //                        );
                //                });
                //    });
                //}

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

                if(Environment.GetEnvironmentVariable("PrivateKeyUri") == null || !File.Exists(Environment.GetEnvironmentVariable("PrivateKeyUri")))
                {
                    //TODO log
                    throw new ArgumentException($"{nameof(Environment.GetEnvironmentVariable)} PrivateKeyUri does not exist.");
                }

                services.AddAggregationHandler(File.ReadAllText(Environment.GetEnvironmentVariable("PrivateKeyUri")));

                // Incoming events will be written to a random file in this directory
                services.AddFileSink(@"C:\ICS");

                // The hosted service executes the micro service
                services.AddHostedService<ICSBackgroundService>();
            });
    }
}
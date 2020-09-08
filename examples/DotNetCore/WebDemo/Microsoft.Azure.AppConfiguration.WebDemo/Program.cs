using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.AppConfiguration.WebDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(builder =>
                    {
                        //
                        // This example uses the Microsoft.Azure.AppConfiguration.AspNetCore NuGet package:
                        // - Establishes the connection to Azure App Configuration using DefaultAzureCredential.
                        // - Loads configuration from Azure App Configuration.
                        // - Sets up dynamic configuration refresh triggered by a sentinel key.

                        // Prerequisite
                        // - An Azure App Configuration store is created
                        // - The application identity is granted "App Configuration Data Reader" role in the App Configuration store
                        // - "AzureAppConfigurationEndpoint" is set to the App Configuration endpoint in either appsettings.json or environment
                        // - The "WebDemo" section in the appsettings.json is imported to the App Configuration store
                        // - A sentinel key "WebDemo:Sentinel" is created in App Configuration to signal the refresh of configuration

                        var settings = builder.Build();
                        string appConfigurationEndpoint = settings["AzureAppConfigurationEndpoint"];
                        string environment = settings["AzureAppConfigurationEnvironment"] ?? "Dev";

                        string appConfigConnectionString = settings["AzureAppConfigurationConnectionStringEnvironment"];

                        builder.AddAzureAppConfiguration(options =>
                        {
                            if (!string.IsNullOrWhiteSpace(appConfigConnectionString))
                                options.Connect(appConfigConnectionString)
                                //options.Connect(new Uri(appConfigurationEndpoint), new DefaultAzureCredential())
                                    //.Select(keyFilter: "WebDemo:*", labelFilter: environment)
                                    //.Select(keyFilter: KeyFilter.Any, labelFilter: environment)
                                    .ConfigureRefresh((refreshOptions) =>
                                    {
                                        // Indicates that all configuration should be refreshed when the given key has changed.
                                        refreshOptions.Register(key: "WebDemo:Sentinel", refreshAll: true);
                                    })
                                    .UseFeatureFlags();
                            //.ConfigureKeyVault(kv =>
                            //    kv.Register(new SecretClient(new Uri("https://nnepal-kv-01.vault.azure.net/"), )));
                            else
                            {
                                if (!string.IsNullOrEmpty(appConfigurationEndpoint))
                                {
                                    options.Connect(new Uri(appConfigurationEndpoint), new DefaultAzureCredential())
                                        .Select(keyFilter: "WebDemo:*", labelFilter: environment)
                                        //.Select(keyFilter: null, labelFilter: environment)
                                        .ConfigureRefresh((refreshOptions) =>
                                        {
                                            // Indicates that all configuration should be refreshed when the given key has changed.
                                            refreshOptions.Register(key: "WebDemo:Sentinel", refreshAll: true);
                                        }).UseFeatureFlags();
                                }


                            }

                        });
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}

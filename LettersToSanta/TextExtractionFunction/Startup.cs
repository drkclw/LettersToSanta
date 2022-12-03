using System;
using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

[assembly: FunctionsStartup(typeof(TextExtractionFunction.Startup))]

namespace TextExtractionFunction
{
    class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            string cs = Environment.GetEnvironmentVariable("TextExtractionConfigCS");
            builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
            {
                options.Connect(cs)
                            .Select(KeyFilter.Any, LabelFilter.Null)
                            .Select(KeyFilter.Any, "Development")
                        .ConfigureKeyVault(kv =>
                        {
                            kv.SetCredential(new DefaultAzureCredential());
                        });
            });
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
        }
    }
}

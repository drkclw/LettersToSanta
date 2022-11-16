using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(TextExtractionFunction.Startup))]

namespace TextExtractionFunction
{
    class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            string cs = Environment.GetEnvironmentVariable("ConnectionString");
            builder.ConfigurationBuilder.AddAzureAppConfiguration(cs);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
        }
    }
}

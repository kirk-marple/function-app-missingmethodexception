using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Unstruk.Functions.Common;

[assembly: FunctionsStartup(typeof(PlatformFunctionsStartup))]

namespace Unstruk.Functions.Common
{
    public class PlatformFunctionsStartup : FunctionsStartupBase
    {
        public PlatformFunctionsStartup()
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            AddConfiguration(services, out string environmentName, out var builder);

            this.Factory = new LoggerFactory();

            AddApplicationInsights();

            this.Logger = Factory.CreateLogger<PlatformFunctionsStartup>();

            if (this.Logger == null)
                throw new InvalidOperationException("Failed to create logger.");

            services.AddSingleton(_ => this.Logger);
            services.AddSingleton(_ => this.Configuration);
            services.AddSingleton(_ => this.TelemetryClient);

            Logger.LogInformation($"Successfully started Azure Functions, environment [{environmentName}].");
        }
    }
}

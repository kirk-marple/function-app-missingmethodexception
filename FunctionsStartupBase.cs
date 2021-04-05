using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Unstruk.Functions.Common
{
    public abstract class FunctionsStartupBase : FunctionsStartup
    {
        public LoggerFactory Factory
        {
            get; protected set;
        }

        public ILogger Logger
        {
            get; protected set;
        }

        public IConfiguration Configuration
        {
            get; protected set;
        }

        public TelemetryClient TelemetryClient
        {
            get; protected set;
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigureServices(builder.Services);
        }

        public abstract void ConfigureServices(IServiceCollection services);

        protected void AddConfiguration(IServiceCollection services, out string environmentName, out IConfigurationBuilder builder)
        {
            const string AZURE_FUNCTIONS_ENVIRONMENT = "AZURE_FUNCTIONS_ENVIRONMENT";

            var provider = services.BuildServiceProvider(); // NOTE: don't dispose

            var executioncontextoptions = provider
                .GetService<IOptions<ExecutionContextOptions>>()?.Value;
            string currentDirectory = executioncontextoptions?.AppDirectory;

            environmentName = Environment.GetEnvironmentVariable(AZURE_FUNCTIONS_ENVIRONMENT);

            builder = new ConfigurationBuilder();

            if (String.IsNullOrEmpty(currentDirectory))
                currentDirectory = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot");

            builder = builder.SetBasePath(currentDirectory);
    
            var existingConfiguration = provider.GetService<IConfiguration>();

            if (existingConfiguration != null)
                builder.AddConfiguration(existingConfiguration);

            builder = builder
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

            this.Configuration = builder.Build();

            services.AddSingleton(this.Configuration);
        }

        protected void AddApplicationInsights()
        {
            string instrumentationKey = Configuration.GetSection("ApplicationInsights")?["InstrumentationKey"];

            if (!String.IsNullOrEmpty(instrumentationKey))
            {
                var tconfiguration = new TelemetryConfiguration
                {
                    InstrumentationKey = instrumentationKey
                };

                tconfiguration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());

                this.TelemetryClient = new TelemetryClient(tconfiguration);
            }
        }
    }
}

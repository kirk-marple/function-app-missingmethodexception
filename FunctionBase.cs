using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Unstruk.Functions.Common
{
    public abstract class FunctionBase
    {
        #region Public fields

        protected readonly ILogger Logger;
        protected readonly IConfiguration Configuration;
        protected readonly TelemetryClient TelemetryClient;

        #endregion

        public FunctionBase(ILogger logger = null, IConfiguration configuration = null, TelemetryClient client = null)
        {
            this.Logger = logger;
            this.Configuration = configuration;
            this.TelemetryClient = client;
        }

        protected Activity GenerateActivity(string operationName, string correlationId)
        {
            return new Activity(operationName).AddTag("correlationId", correlationId);
        }
    }
}

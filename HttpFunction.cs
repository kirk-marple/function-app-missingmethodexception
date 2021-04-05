using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unstruk.Functions.Common
{
    public class HttpFunction : FunctionBase
    {
        public HttpFunction(
            ILogger logger = null, IConfiguration configuration = null, TelemetryClient client = null) :
            base(logger, configuration, client)
        {
            Logger.LogDebug("Initialized HTTP function.");
        }

        [FunctionName("HttpFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest request)
        {
            using var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            string correlationId = Guid.NewGuid().ToString();

            using var dop = TelemetryClient.StartOperation<DependencyTelemetry>(GenerateActivity("HTTP request", correlationId));

            try
            {
                Logger.LogInformation($"[{correlationId}] Successfully handled HTTP request.");

                dop.Telemetry.Success = true;
            }
            catch (Exception e)
            {
                dop.Telemetry.Success = false;

                Logger.LogError($"[{correlationId}] Failed to handle HTTP request. {e.Message}");
            }

            return new OkResult();
        }
    }
}

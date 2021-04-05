To reproduce this problem, run the function app in the debugger.

Once loaded, hit http://localhost:7071/api/HttpFunction

This results in the error:

``` shell
[2021-04-05T21:01:48.941Z] System.Private.CoreLib: Exception while executing function: HttpFunction. FunctionApp: Method not found: 'Microsoft.ApplicationInsights.Extensibility.IOperationHolder`1<!!0> Microsoft.ApplicationInsights.TelemetryClientExtensions.StartOperation(Microsoft.ApplicationInsights.TelemetryClient, System.Diagnostics.Activity)'.
```

If you change the version to 2.15.0, rebuild and rerun, the error doesn't occur when hitting the HTTP endpoint.

``` shell
        <PackageReference Include="Microsoft.ApplicationInsights.AspNetCore" Version="2.15.0" />
```

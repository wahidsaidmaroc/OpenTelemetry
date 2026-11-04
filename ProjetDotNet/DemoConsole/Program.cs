using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string serviceName = "DemoConsole";
const string serviceVersion = "1.0.0";

using var activitySource = new ActivitySource(serviceName);
using var meter = new Meter(serviceName);

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion: serviceVersion))
    .AddSource(serviceName)
    .AddConsoleExporter()
    .Build();

using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion: serviceVersion))
    .AddMeter(serviceName)
    .AddConsoleExporter()
    .Build();

using var loggerFactory = LoggerFactory.Create(builder =>
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion: serviceVersion));
        options.IncludeFormattedMessage = true;
        options.AddConsoleExporter();
    }));

var logger = loggerFactory.CreateLogger(serviceName);
var operationCounter = meter.CreateCounter<int>("demo.operations", unit: "{operation}");

using (var activity = activitySource.StartActivity("DemoOperation"))
{
    activity?.SetTag("demo.operation", "say_hello");
    logger.LogInformation("Debut de l'operation de demonstration.");

    Console.WriteLine("Bonjour depuis DemoConsole avec OpenTelemetry !");
    operationCounter.Add(1);

    activity?.SetStatus(ActivityStatusCode.Ok);
    logger.LogInformation("Operation terminee avec succes.");
}

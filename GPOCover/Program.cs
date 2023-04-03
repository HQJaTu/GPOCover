using GPOCover;
using GPOCover.Cover;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

#pragma warning disable 1416


void Configure(HostApplicationBuilder builder)
{
    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = "GPO Cover Service";
    });

    LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

    builder.Services.AddSingleton<CoverService>();
    builder.Services.AddHostedService<WindowsService>();

    var env = builder.Environment;

    var currentEnvironmentConfiguration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .Build();

    var productionConfiguration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"appsettings.json", optional: false)
        .Build();

    builder.Configuration
        .AddConfiguration(productionConfiguration)
        .AddConfiguration(currentEnvironmentConfiguration);

    // See: https://github.com/dotnet/runtime/issues/47303
    builder.Logging.AddConfiguration(
        builder.Configuration.GetSection("Logging"));
}


var builder = Host.CreateApplicationBuilder(args);
Configure(builder);
var host = builder.Build();
host.Run();

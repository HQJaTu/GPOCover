using GPOCover;
using GPOCover.Cover;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using System.CommandLine;
using CliWrap;
using System.Diagnostics;
using System.ServiceProcess;

const string ServiceName = "GPOCover"; // see: appsettings.json, EventLog, SourceName for Event Log logging with this application name
const string ServiceDisplayName = "GPO Cover Service";

#pragma warning disable 1416

void Configure(HostApplicationBuilder builder)
{
    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = ServiceName;
    });

    LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

    builder.Services.AddSingleton<CoverService>();
    builder.Services.AddHostedService<WindowsService>();

    var env = builder.Environment;

    var currentEnvironmentConfiguration = new ConfigurationBuilder()
        .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .Build();

    var productionConfiguration = new ConfigurationBuilder()
        .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile($"appsettings.json", optional: false)
        .Build();

    builder.Configuration
        .AddConfiguration(productionConfiguration)
        .AddConfiguration(currentEnvironmentConfiguration);

    // See: https://github.com/dotnet/runtime/issues/47303
    builder.Logging.AddConfiguration(
        builder.Configuration.GetSection("Logging"));
}


/**
 * Run modes (whould be Enum, but cannot define inside wrapped main():
 * 0 = Don't know!
 * 1 = Run
 * 2 = Install
 * 3 = Uninstall
 */
async Task<int> ParseArgs(string[] args)
{
    int operationMode = 0;
    var rootCommand = new RootCommand();
    var runCommand = new System.CommandLine.Command("run", "Run Windows Service. Default mode.");
    var installCommand = new System.CommandLine.Command("/Install", "Install Windows Service");
    installCommand.AddAlias("/install");
    installCommand.AddAlias("--install");
    var uninstallCommand = new System.CommandLine.Command("/Uninstall", "Stop and unnstall Windows Service");
    uninstallCommand.AddAlias("/uninstall");
    uninstallCommand.AddAlias("--uninstall");

    runCommand.SetHandler(() =>
    {
        operationMode = 1;
        Console.WriteLine("Command: Running service");
    });
    installCommand.SetHandler(() =>
    {
        operationMode = 2;
        Console.WriteLine("Command: Installing service");
    });
    uninstallCommand.SetHandler(() =>
    {
        operationMode = 3;
        Console.WriteLine("Command: Uninstalling service");
    });
    rootCommand.Add(runCommand);
    rootCommand.Add(installCommand);
    rootCommand.Add(uninstallCommand);

    // fallback to default:
    if (args.Length == 0)
        args = new string[] { "run" };
    await rootCommand.InvokeAsync(args);

    return operationMode; 
}

FileInfo GetExecutablePath()
{
    var appPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, System.AppDomain.CurrentDomain.FriendlyName);
    var processName = Process.GetCurrentProcess().MainModule?.FileName;
    if (processName is null)
        throw new ArgumentNullException(nameof(processName));

    var info = new FileInfo(processName);
    if (info is null)
        throw new ArgumentNullException(nameof(info));
#if DEBUG
    if (!info.Exists)
        throw new ArgumentOutOfRangeException("Internal: Process executable file not found!");
#endif

    return info;
}


//
// Begin execution
//

var operationMode = await ParseArgs(args);
switch (operationMode) {
    case 1:
        var builder = Host.CreateApplicationBuilder(args);
        Configure(builder);
        var host = builder.Build();
        host.Run();
        break;

    case 2:
        var ctl = System.ServiceProcess.ServiceController.GetServices()
            .FirstOrDefault(s => s.ServiceName == ServiceName);
        if (ctl is null)
            // Docs: https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/sc-create
            await Cli.Wrap("sc.exe")
                .WithArguments(new[] {
                    "create",
                    ServiceName,
                    "type=own",
                    $"binPath={GetExecutablePath().FullName}",
                    "start=auto",
                    $"DisplayName={ServiceDisplayName}",
                    "obj=LocalSystem"
                })
                .ExecuteAsync();
        else
            Console.WriteLine($"Windows Service '{ServiceName}' already exists. Skip Windows Service creation.");
        break;

    case 3:
        // Docs: https://github.com/Tyrrrz/CliWrap
        Console.WriteLine($"Bin: {GetExecutablePath().FullName}");
        await Cli.Wrap("sc.exe")
            .WithArguments(new[] { "stop", ServiceName })
            .WithValidation(CommandResultValidation.None) // Ignore possible failure on stopping the service
            .ExecuteAsync();

        await Cli.Wrap("sc.exe")
            .WithArguments(new[] { "delete", ServiceName })
            .ExecuteAsync();
        break;

    default:
        Console.WriteLine($"Exit. No operation chosen ({operationMode}).");
        break;
}

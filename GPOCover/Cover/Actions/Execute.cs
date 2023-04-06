using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GPOCover.Cover.Actions;

internal class Execute : ActionBase
{
    internal string command;
    internal string? arguments;
    protected readonly ILogger<Noop> _logger;

    internal Execute(string command, string? arguments, ILoggerFactory loggerFactory)
    {
        this.command = command;
        this.arguments = arguments;
        _logger = loggerFactory.CreateLogger<Noop>();
    }

    override public async Task RunAsync()
    {
        this._logger.LogInformation($"Executing command: {this.command}!");
        int exitStatus = await Execute.RunProcessAsync(this.command, this.arguments, this._logger);
        if (exitStatus > 0)
            this._logger.LogError("Executing command failed with exit code: {exitStatus}", exitStatus);
        else
            this._logger.LogInformation($"Executed command ok.");
    }

    static Task<int> RunProcessAsync(string fileName, string? arguments, ILogger logger)
    {
        // See: https://github.com/jamesmanning/RunProcessAsTask
        var tcs = new TaskCompletionSource<int>();

        var process = new Process
        {
            StartInfo = {
                FileName = fileName,
                Arguments = arguments,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
            },
            EnableRaisingEvents = true
        };

        process.Exited += (sender, args) =>
        {
            var senderProcess = sender as Process;
            if (senderProcess != null)
            {
                var stdout = senderProcess.StandardOutput.ReadToEnd();
                logger.LogDebug($"Standard output of execution: {stdout}");
            }
            tcs.SetResult(process.ExitCode);
            process.Dispose();
        };

        process.Start();

        return tcs.Task;
    }

}

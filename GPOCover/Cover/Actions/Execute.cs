using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPOCover.Cover.Actions;

internal class Execute : ActionBase
{
    internal string command;
    protected readonly ILogger<Noop> _logger;

    internal Execute(string command, ILoggerFactory loggerFactory)
    {
        this.command = command;
        _logger = loggerFactory.CreateLogger<Noop>();
    }

    override public async Task RunAsync()
    {
        this._logger.LogInformation($"Executing command: {this.command}!");
        int exitStatus = await Execute.RunProcessAsync(this.command, null);
        if (exitStatus > 0)
            this._logger.LogError($"Executing command failed with exit code: {exitStatus}");
        else
            this._logger.LogInformation($"Executed command ok.");
    }

    static Task<int> RunProcessAsync(string fileName, string? arguments)
    {
        // See: https://github.com/jamesmanning/RunProcessAsTask
        var tcs = new TaskCompletionSource<int>();

        var process = new Process
        {
            StartInfo = {
                FileName = fileName,
                Arguments = arguments,
                WindowStyle = ProcessWindowStyle.Hidden,
            },
            EnableRaisingEvents = true
        };

        process.Exited += (sender, args) =>
        {
            tcs.SetResult(process.ExitCode);
            process.Dispose();
        };

        process.Start();

        return tcs.Task;
    }

}

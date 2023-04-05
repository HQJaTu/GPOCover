using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPOCover.Cover.Actions;

internal class Sleep : ActionBase
{
    protected uint _sleepMilliSeconds;
    protected readonly ILogger<Noop> _logger;

    internal Sleep(uint sleepMilliSeconds, ILoggerFactory loggerFactory)
    {
        this._sleepMilliSeconds = sleepMilliSeconds;
        _logger = loggerFactory.CreateLogger<Noop>();
    }

    public override async Task RunAsync()
    {
        this._logger.LogInformation($"Sleeping for {this._sleepMilliSeconds} milliseconds");
        await Task.Delay((int)this._sleepMilliSeconds);
    }
}

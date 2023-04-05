using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPOCover.Cover.Actions;

internal class Noop : ActionBase
{
    protected readonly ILogger<Noop> _logger;

    internal Noop(ILoggerFactory loggerFactory) :
        base()
    {
        _logger = loggerFactory.CreateLogger<Noop>();
    }

    override public async Task RunAsync()
    {
        this._logger.LogInformation($"Running a no-operation!");
        await Task.Delay(1);
    }

}

using GPOCover.Cover.Actions;
using GPOCover.Cover.Configuration;
using GPOCover.Cover.Triggers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPOCover.Cover;

internal static class CoverServiceConfigurator
{
    static public (List<TriggerRegistryChange>,
        List<TriggerDirectoryChange>,
        List<LockFile>) Configure(List<CoverConfiguration> configIn, ILoggerFactory loggerFactory)
    {
        var registryTriggers = configIn
            .Select((c, idx) => (idx, c)) // convert list of configurations into tuple with 0-based index in list
            .Where(ci => ci.c.Trigger.Type == Trigger.RegistryChange) // filter
            .Select(ci => AddRegistryChangeWatch((uint)ci.idx + 1, ci.c, loggerFactory)) // transform all matching
            .Distinct() // weed out any possible duplicates
            .ToList();
        var directoryTriggers = configIn
            .Select((c, idx) => (idx, c))
            .Where(ci => ci.c.Trigger.Type == Trigger.FilesystemChange)
            .Select(ci => AddDirectoryChangeWatch((uint)ci.idx + 1, ci.c, loggerFactory))
            .Distinct()
            .ToList();
        var fileLocks = configIn
            .Select((c, idx) => (idx, c))
            .Where(ci => ci.c.Trigger.Type == Trigger.FilesystemLock)
            .Select(ci => AddFileLockWatch((uint)ci.idx + 1, ci.c, loggerFactory))
            .Distinct()
            .ToList();

        return (registryTriggers, directoryTriggers, fileLocks);
    } // end Configure()

    internal static TriggerRegistryChange AddRegistryChangeWatch(uint configurationId, CoverConfiguration config, ILoggerFactory loggerFactory)
    {
        if (config.Trigger is null)
            throw new ArgumentNullException(nameof(config.Trigger));
        if (config.Trigger.Key is null)
            throw new ArgumentNullException(nameof(config.Trigger.Key));
        if (config.Trigger.Condition is not null && !String.IsNullOrEmpty(config.Trigger.Condition.PathExists))
            throw new ArgumentException($"RegistryChange-trigger '{config.Name}' cannot have 'PathExists'-condition!");

        string? registryKeyExists = null;
        string? registryValueExists = null;
        if (config.Trigger.Condition is not null && !String.IsNullOrEmpty(config.Trigger.Condition.KeyExists))
            registryKeyExists = config.Trigger.Condition.KeyExists;
        if (config.Trigger.Condition is not null && !String.IsNullOrEmpty(config.Trigger.Condition.ValueExists))
            registryValueExists = config.Trigger.Condition.ValueExists;
        var trigger = new TriggerRegistryChange(configurationId, config.Trigger.Key,
            registryKeyExists, registryValueExists, 
            loggerFactory);
        trigger.AddActions(config.Actions.Select(a => _convertAction(a, config, loggerFactory)).ToList<ActionBase>());
        trigger.Start();

        return trigger;
    }

    internal static TriggerDirectoryChange AddDirectoryChangeWatch(uint configurationId, CoverConfiguration config, ILoggerFactory loggerFactory)
    {
        if (config.Trigger is null)
            throw new ArgumentNullException(nameof(config.Trigger));
        if (config.Trigger.Path is null)
            throw new ArgumentNullException(nameof(config.Trigger.Path));
        if (config.Trigger.Condition is not null && !String.IsNullOrEmpty(config.Trigger.Condition.ValueExists))
            throw new ArgumentException($"FilesystemChange-trigger '{config.Name}' cannot have 'ValueExists'-condition!");

        var dirInfo = new DirectoryInfo(config.Trigger.Path);
        string? pathExists = null;
        if (config.Trigger.Condition is not null && !String.IsNullOrEmpty(config.Trigger.Condition.PathExists))
            pathExists = config.Trigger.Condition.PathExists;
        var trigger = new TriggerDirectoryChange(configurationId, dirInfo, pathExists, loggerFactory);
        trigger.AddActions(config.Actions.Select(a => _convertAction(a, config, loggerFactory)).ToList<ActionBase>());
        trigger.Start();

        return trigger;
    }

    internal static LockFile AddFileLockWatch(uint configurationId, CoverConfiguration config, ILoggerFactory loggerFactory)
    {
        if (config.Trigger is null)
            throw new ArgumentNullException(nameof(config.Trigger));
        if (config.Trigger.Path is null)
            throw new ArgumentNullException(nameof(config.Trigger.Path));

        var fileInfo = new FileInfo(config.Trigger.Path);
        var lockFile = new LockFile(configurationId, fileInfo, loggerFactory);
        if (config.Actions is not null)
            lockFile.AddActions(config.Actions.Select(a => _convertAction(a, config, loggerFactory)).ToList<ActionBase>());
        lockFile.Start();

        return lockFile;
    }

    internal static ActionBase _convertAction(CoverConfigurationAction action, CoverConfiguration config, ILoggerFactory loggerFactory)
    {
        if (action.Noop is not null)
            return new Noop(loggerFactory);
        if (action.Execute is not null)
        {
            if (String.IsNullOrEmpty(action.Execute.Command))
                throw new ArgumentException($"Execute-action for '{config.Name}' needs to have a command!");

            return new Execute(action.Execute.Command, action.Execute.Arguments, loggerFactory);
        }
        if (action.Sleep is not null)
            return new Sleep(action.Sleep.Value, loggerFactory);

        throw new NotImplementedException($"Unknown action type in YAML!");
    }

} // end class CoverServiceConfigurator

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPOCover.FileUtils;
using GPOCover.RegistryUtils;

namespace GPOCover.Cover.Triggers;

internal class TriggerDirectoryChange : TriggerBase
{
    protected DirectoryInfo _directoryInfo;
    protected FilesystemDirectoryChange _trigger;

    protected readonly ILogger<TriggerDirectoryChange> _logger;

    public TriggerDirectoryChange(uint id, DirectoryInfo directoryInfo, ILoggerFactory loggerFactory) :
        base(id)
    {
        _logger = loggerFactory.CreateLogger<TriggerDirectoryChange>();
        _directoryInfo = directoryInfo;
        _trigger = new FilesystemDirectoryChange(_directoryInfo);
        _trigger.AddChangeCallback(OnChange);
        _trigger.AddRenameCallback(OnRename);
        _trigger.AddErrorCallback(OnError);
    }

    public void OnChange(object sender, FileSystemEventArgs e)
    {
        _logger.LogWarning($"Trigger {this.Id}: Directory: {_directoryInfo.FullName}, has changed");
        this.RunActions();
    }

    public void OnRename(object sender, RenamedEventArgs e)
    {
        _logger.LogWarning($"Trigger {this.Id}: Directory: {_directoryInfo.FullName}, was renamed");
        this.RunActions();
    }

    public void OnError(object sender, ErrorEventArgs e)
    {
        _logger.LogError($"Trigger {this.Id}: Directory: {_directoryInfo.FullName}, failed");
}

} // end class TriggerDirectoryChange

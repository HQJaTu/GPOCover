using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPOCover.FileUtils;
using GPOCover.RegistryUtils;

namespace GPOCover.Cover;

internal class TriggerDirectoryChange
{
    protected DirectoryInfo _directoryInfo;
    protected FilesystemDirectoryChange _trigger;

    protected readonly ILogger<TriggerDirectoryChange> _logger;

    public TriggerDirectoryChange(DirectoryInfo directoryInfo, ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<TriggerDirectoryChange>();
        _directoryInfo = directoryInfo;
        _trigger = new FilesystemDirectoryChange(_directoryInfo);
        _trigger.AddChangeCallback(OnChange);
        _trigger.AddRenameCallback(OnRename);
        _trigger.AddErrorCallback(OnError);
    }

    public void OnChange(object sender, FileSystemEventArgs e)
    {
        _logger.LogWarning($"Directory: {this._directoryInfo.FullName}, has changed");
    }

    public void OnRename(object sender, RenamedEventArgs e)
    {
        _logger.LogWarning($"Directory: {this._directoryInfo.FullName}, was renamed");
    }

    public void OnError(object sender, ErrorEventArgs e)
    {
        _logger.LogError($"Directory: {this._directoryInfo.FullName}, failed");
    }


}

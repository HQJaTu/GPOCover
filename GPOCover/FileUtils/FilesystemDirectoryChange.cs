using Microsoft.Extensions.FileSystemGlobbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GPOCover.FileUtils;

internal class FilesystemDirectoryChange : FilesystemChangeBase
{
    public FilesystemDirectoryChange(DirectoryInfo directoryInfo)
    {
        CreateWatcher(directoryInfo.FullName);

        _watcher.NotifyFilter = NotifyFilters.Attributes
                     | NotifyFilters.CreationTime
                     | NotifyFilters.DirectoryName
                     | NotifyFilters.FileName
                     //| NotifyFilters.LastAccess
                     | NotifyFilters.LastWrite
                     | NotifyFilters.Security
                     | NotifyFilters.Size;

        //_watcher.Filter = "*.txt";
        _watcher.IncludeSubdirectories = true;
        _watcher.EnableRaisingEvents = true;
    }

    public void AddChangeCallback(FileSystemEventHandler callback)
    {
        _watcher.Changed += callback;
        _watcher.Created += callback;
        _watcher.Deleted += callback;
    }

    public void AddRenameCallback(RenamedEventHandler callback)
    {
        _watcher.Renamed += callback;
    }

    public void AddErrorCallback(ErrorEventHandler callback)
    {
        _watcher.Error += callback;
    }

}

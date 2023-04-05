using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPOCover.Cover.Triggers;

internal class LockFile : TriggerBase
{
    protected FileInfo _fileInfo;
    protected FileStream? _locked = null;

    protected readonly ILogger<LockFile> _logger;

    public LockFile(FileInfo fileInfo, ILoggerFactory loggerFactory) :
        base()
    {
        _logger = loggerFactory.CreateLogger<LockFile>();
        _fileInfo = fileInfo;

        DoLockTheFile();
    }

    protected void DoLockTheFile()
    {
        if (_fileInfo is null)
            throw new ArgumentNullException(nameof(_fileInfo));
        if (_fileInfo.DirectoryName is null)
            throw new ArgumentException(nameof(_fileInfo.Directory));
        if (!Directory.Exists(_fileInfo.DirectoryName))
        {
            var di = Directory.CreateDirectory(_fileInfo.DirectoryName);
        }

        _locked = File.Open(_fileInfo.FullName,
            FileMode.OpenOrCreate,
            FileAccess.Write,
            FileShare.None);
        _logger.LogInformation($"Locked file: {_fileInfo.FullName}");
    }

} // end class LockFile

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPOCover.Cover;

internal class LockFile
{
    protected FileInfo _fileInfo;
    protected FileStream? _locked = null;

    protected readonly ILogger<LockFile> _logger;

    public LockFile(FileInfo fileInfo, ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<LockFile>();
        _fileInfo = fileInfo;

        DoLockTheFile();
    }

    protected void DoLockTheFile()
    {
        if (this._fileInfo is null)
            throw new ArgumentNullException(nameof(this._fileInfo));
        if (this._fileInfo.DirectoryName is null)
            throw new ArgumentException(nameof(this._fileInfo.Directory));
        if (!Directory.Exists(this._fileInfo.DirectoryName)) {
            var di = Directory.CreateDirectory(this._fileInfo.DirectoryName);
        }

        this._locked = File.Open(this._fileInfo.FullName,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);
        this._logger.LogInformation($"Locked file: {this._fileInfo.FullName}");
    }

}

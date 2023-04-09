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
    protected FilesystemDirectoryChange _trigger;
    protected DirectoryInfo DirectoryToWatch { get; set; }
    protected string? CheckPathExists { get; set; }

    protected readonly ILogger<TriggerDirectoryChange> _logger;

    public TriggerDirectoryChange(uint id, DirectoryInfo directoryInfo, string? checkPathExists, ILoggerFactory loggerFactory) :
        base(id)
    {
        _logger = loggerFactory.CreateLogger<TriggerDirectoryChange>();

        this.DirectoryToWatch = directoryInfo;
        this.CheckPathExists = checkPathExists;
        this._trigger = new FilesystemDirectoryChange(this.DirectoryToWatch);
    }

    public override void Start()
    {
        if (this.CheckPathExists is not null && this.CheckIfPathExists())
            this.ConditionallyRunActions();

        _trigger.AddChangeCallback(OnChange);
        _trigger.AddRenameCallback(OnRename);
        _trigger.AddErrorCallback(OnError);
    }

    public void OnChange(object sender, FileSystemEventArgs e)
    {
        _logger.LogWarning($"Trigger {this.Id}: Directory: {this.DirectoryToWatch.FullName}, has changed");
        this.ConditionallyRunActions();
    }

    public void OnRename(object sender, RenamedEventArgs e)
    {
        _logger.LogWarning($"Trigger {this.Id}: Directory: {this.DirectoryToWatch.FullName}, was renamed");
        this.ConditionallyRunActions();
    }

    protected void ConditionallyRunActions()
    {
        if (this.CheckPathExists is null)
            this.RunActions();
        else if (this.CheckIfPathExists())
            this.RunActions();
    }

    protected bool CheckIfPathExists()
    {
        if (this.CheckPathExists is null)
            throw new ArgumentNullException(nameof(this.CheckPathExists));
        var dirCheck = new DirectoryInfo(Path.Combine(DirectoryToWatch.FullName, this.CheckPathExists));
        var fileCheck = new FileInfo(Path.Combine(DirectoryToWatch.FullName, this.CheckPathExists));

        if (dirCheck.Exists)
            return true;
        if (fileCheck.Exists)
            return true;

        return false;
    }

    public void OnError(object sender, ErrorEventArgs e)
    {
        _logger.LogError($"Trigger {this.Id}: Directory: {this.DirectoryToWatch.FullName}, failed");
}

} // end class TriggerDirectoryChange

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPOCover.RegistryUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;


namespace GPOCover.Cover.Triggers;
#pragma warning disable 1416

internal class TriggerRegistryChange : TriggerBase
{
    protected RegistryKeyChange _keyChange;
    protected RegistryKey Hive { get; set; }
    protected string KeyPath { get; set; }
    protected string? CheckKeyExists { get; set; }
    protected string? CheckValueExists { get; set; }

    protected readonly ILogger<TriggerRegistryChange> _logger;

    public TriggerRegistryChange(uint id, string path,
        string? checkKeyExists, string? checkValueExists,
        ILoggerFactory loggerFactory) :
        base(id)
    {
        _logger = loggerFactory.CreateLogger<TriggerRegistryChange>();
        (Hive, KeyPath) = Parse(path);
        CheckKeyExists = checkKeyExists;
        CheckValueExists = checkValueExists;

        _keyChange = new RegistryKeyChange(Hive, KeyPath, loggerFactory);
        _keyChange.RegistryKeyChanged += new EventHandler<RegistryKeyChangedEventArgs>(OnRegChanged);
    }

    public override void Start()
    {
        var key = this.Hive.OpenSubKey(this.KeyPath);
        if (key is null)
            throw new ArgumentException($"RegistryChange trigger for {Hive.Name}\\{KeyPath} fails! Key doesn't exist.");

        if (CheckKeyExists is not null && this.CheckIfKeyExists())
            this.RunActions();

        this._keyChange.Start();
    }

    protected void OnRegChanged(object? sender, RegistryKeyChangedEventArgs e)
    {
        _logger.LogWarning($"Trigger {this.Id}: Registry key: {Hive.Name}\\{KeyPath}, has changed");

        if (this.CheckKeyExists is null && this.CheckValueExists is null) 
            this.RunActions();
        else if (this.CheckKeyExists is not null && this.CheckIfKeyExists())
            this.RunActions();
        else if (this.CheckValueExists is not null && this.CheckIfValueExists())
            this.RunActions();
    }

    protected bool CheckIfKeyExists()
    {
        try
        {
            using (var key = this.Hive.OpenSubKey(this.KeyPath))
            {
                if (key is not null)
                {
                    if (key.GetSubKeyNames().Contains(this.CheckKeyExists))
                        return true;
                }
            }
        }
        catch (Exception)
        {
            this._logger.LogError($"Exception while reading registry key '{this.Hive.Name}\\{this.KeyPath}'. Ignoring error.");

            return false;
        }

        return false;
    }

    protected bool CheckIfValueExists()
    {
        try
        {
            using (var key = this.Hive.OpenSubKey(this.KeyPath))
            {
                if (key is not null)
                {
                    if (key.GetValueNames().Contains(this.CheckValueExists))
                        return true;
                }
            }
        }
        catch (Exception)
        {
            this._logger.LogError($"Exception while reading registry key '{this.Hive.Name}\\{this.KeyPath}'. Ignoring error.");

            return false;
        }

        return false;
    }

    protected static (RegistryKey, string) Parse(string path)
    {
        var parts = path.Split('\\', 2);
        var hive = parts[0] switch
        {
            //"HKCR" => Registry.ClassesRoot,
            "HKCR" => throw new ArgumentException($"Invalid trigger on hive {parts[0]}. Use 'HKLM\\SOFTWARE\\Classes' instead!"),
            //"HKCU" => Registry.CurrentUser,
            "HKCU" => throw new ArgumentException($"Invalid trigger on hive {parts[0]}. Use 'HKU\\<user SAM id>' instead!"),
            "HKLM" => Registry.LocalMachine,
            "HKU" => Registry.Users,
            //"HKPD" => Registry.PerformanceData,
            "HKPD" => throw new ArgumentException($"Invalid trigger on hive {parts[0]}. Unsupported!"),
            "HKCC" => Registry.CurrentConfig,
            _ => throw new ArgumentException($"Unknown hive {parts[0]}. Cannot continue!")
        };
        var wmiPath = parts[1].Replace(@"\", @"\\");

        return (hive, wmiPath);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not TriggerRegistryChange)
            return false;
        var registryObj = obj as TriggerRegistryChange;
        if (registryObj is null)
            return false;
        if (Hive != registryObj.Hive)
            return false;
        if (KeyPath != registryObj.KeyPath)
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

} // class TriggerRegistry

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPOCover.RegistryUtils;
using Microsoft.Win32;


namespace GPOCover.Cover.Triggers;
#pragma warning disable 1416

internal class TriggerRegistryChange : TriggerBase
{
    protected RegistryKey Hive { get; set; }
    protected string KeyPath { get; set; }

    protected readonly ILogger<TriggerRegistryChange> _logger;

    public TriggerRegistryChange(uint id, string path, ILoggerFactory loggerFactory) :
        base(id)
    {
        _logger = loggerFactory.CreateLogger<TriggerRegistryChange>();
        (Hive, KeyPath) = Parse(path);
        var keychange = new RegistryKeyChange(Hive, KeyPath, loggerFactory);
        keychange.RegistryKeyChanged += new EventHandler<RegistryKeyChangedEventArgs>(OnRegChanged);
        keychange.Start();
    }

    private void OnRegChanged(object? sender, RegistryKeyChangedEventArgs e)
    {
        _logger.LogWarning($"Trigger {this.Id}: Registry key: {Hive.Name}\\{KeyPath}, has changed");
        this.RunActions();
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

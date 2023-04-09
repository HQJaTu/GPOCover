using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace GPOCover.Cover.Configuration;

public enum Trigger : ushort
{
    None = 0,
    Unknown = 1,
    RegistryChange = 100,
    FilesystemChange,
    FilesystemLock
}

public enum ActionType : ushort
{
    None = 0,
    Unknown = 1,
    Noop = 2,
    Execute = 100,
    Sleep
}

public class CoverConfiguration
{
    [YamlMember(Alias = "Name", ApplyNamingConventions = false)]
    public string? Name { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [YamlMember(Alias = "Trigger", ApplyNamingConventions = false)]
    public CoverConfigurationTrigger Trigger { get; set; }
    [YamlMember(Alias = "Actions", ApplyNamingConventions = false)]
    public List<CoverConfigurationAction> Actions { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public class CoverConfigurationTrigger
{
    [YamlMember(Alias = "Type", ApplyNamingConventions = false)]
    public Trigger Type { get; set; }
    [YamlMember(Alias = "Path", ApplyNamingConventions = false)]
    public string? Path { get; set; }
    [YamlMember(Alias = "Key", ApplyNamingConventions = false)]
    public string? Key { get; set; }
    [YamlMember(Alias = "Condition", ApplyNamingConventions = false)]
    public CoverConfigurationTriggerCondition? Condition { get; set; }
}

public class CoverConfigurationTriggerCondition
{
    [YamlMember(Alias = "KeyExists", ApplyNamingConventions = false)]
    public string? KeyExists { get; set; }
    [YamlMember(Alias = "ValueExists", ApplyNamingConventions = false)]
    public string? ValueExists { get; set; }
    [YamlMember(Alias = "PathExists", ApplyNamingConventions = false)]
    public string? PathExists { get; set; }
}

public class CoverConfigurationAction
{
    [YamlMember(Alias = "Noop", ApplyNamingConventions = false)]
    public bool? Noop { get; set; }
    [YamlMember(Alias = "Sleep", ApplyNamingConventions = false)]
    public uint? Sleep { get; set; }
    [YamlMember(Alias = "Execute", ApplyNamingConventions = false)]
    public CoverConfigurationActionExecute? Execute { get; set; }
}

public class CoverConfigurationActionExecute
{
    [YamlMember(Alias = "Command", ApplyNamingConventions = false)]
    public string? Command { get; set; }
    [YamlMember(Alias = "Arguments", ApplyNamingConventions = false)]
    public string? Arguments { get; set; }
}

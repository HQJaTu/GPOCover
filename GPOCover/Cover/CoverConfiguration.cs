using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace GPOCover.Cover;

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
    Execute = 100
}

public class CoverConfiguration
{
    [YamlMember(Alias = "Name", ApplyNamingConventions = false)]
    public string? Name { get; set; }
    [YamlMember(Alias = "Trigger", ApplyNamingConventions = false)]
    public Trigger Trigger { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [YamlMember(Alias = "Path", ApplyNamingConventions = false)]
    public string Path { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [YamlMember(Alias = "Check-on-start", ApplyNamingConventions = false)]
    public bool? Check_on_start { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [YamlMember(Alias = "Actions", ApplyNamingConventions = false)]
    public List<CoverConfigurationAction> Actions { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public class CoverConfigurationAction {
    [YamlMember(Alias = "Noop", ApplyNamingConventions = false)]
    public bool? Noop { get; set; }
    [YamlMember(Alias = "Sleep", ApplyNamingConventions = false)]
    public uint? Sleep { get; set; }
    [YamlMember(Alias = "Execute", ApplyNamingConventions = false)]
    public string? Execute { get; set; }
}
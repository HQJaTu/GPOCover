# GPOCover
Windows service to protect you from GPO-originating changes

## Configuration file
All configuration files must be stored into `%programdata%\GPOCover`.

Any number of `.yaml`-files can be used. A single trigger will be processed from a single YAML-file.

### Configuration file format
Base format:
```yaml
%YAML 1.1
---
Name: The trigger with a name
Trigger:
    Type: typeOfThisTrigger
Actions:
    - ListOfActions: optionally, yes
```

* `Name`: Name / Description of this trigger. Name will be used for logging events.
* `Trigger`: What to monitor. A single trigger will be monitored.
* `Actions`: What to do when trigger event occurs. Any number of actions can be applied. None is also ok.

### Trigger: RegistryChange
```yaml
Trigger:
    Type: RegistryChange
    Key: HKU\S-1-5-21\SOFTWARE\Bar
    Condition:
        ValueExists: foo
```

Monitor a registry change in an existing Registry key. Optional condition can be applied.

In Windows, it is possible to monitor following hives via WMI:
* `HKLM` / LocalMachine
* `HKU` / Users
* `HKCC` / CurrentConfig

In Windows, it is *not* possible to monitor following hives:
* If wanting to monitor `HKCR` / ClassesRoot, do that via `HKLM\SOFTWARE\Classes`.
* If wanting to monitor `HKCU` / CurrentUser, there is no "current" nor "user" in a Windows Service running as SYSTEM.
  Do this via `HKU\<user's SAM id>\`.

#### (optional) Condition
As any change in a given key or its subkeys will trigger, sometimes further inspection will be needed for not to run actions unnecessarily.

To run actions only after a registry:
* sub-key exists, use `KeyExists`
* value exists, use `ValueExists`

If no `KeyExists` nor `ValueExists` is specified, any change in a given registry key will trigger actions.

Condition is checked on service start. If no condition is specified, no checking is done on service start.

### Trigger: FilesystemChange
```yaml
Trigger:
    Type: FilesystemChange
    Path: C:\A path\in the system
    Condition:
        ValueExists: foo
```

Monitor file system. Trigger on any change.

#### (optional) Condition
As any change in a given path or its subdirectories will trigger, sometimes further inspection will be needed for not to run actions unnecessarily.

To run actions only after a specific directory or file exists, use `PathExists`

Optional condition is checked on service start. If no condition is specified, no checking is done on service start.

### Trigger: FilesystemLock
```yaml
Trigger:
    Type: FilesystemLock
    Path: C:\A path\in the system\file.lock
```

Create a file into given path and keep it open for writing. While file is kept open, there is an access lock in it.
Nobody else, besides GPO Cover service can access the file.

This effectively prevents other processes, like installers, from creating and writing into the locked file.
Any `setup.exe` attempting to create/write is doomed to fail!

### Action: Noop
```yaml
Actions:
    - Noop: true
```

Do absolutely nothing. Action will be logged.

### Action: Sleep
```yaml
Actions:
    - Sleep: 300
```

Pause execution for amount of milliseconds [*ms*]. Sometimes acting instantly on a trigger is *too-fast-4-you* and bit of delay must be applied.

### Action: Execute
```yaml
Actions:
    - Execute:
        Command: C:\Windows\system32\reg.exe
        Arguments: delete "HKU\S-1-5-21\SOFTWARE\Bar" /v foo /f
```

Run a command. Arguments are optional.

## Deployment project with `.msi`
Project for creating deployment package `GPOCover.Setup` will require a Visual Studio extension to operate.
A Windows Installer MSI will be created. More info: https://learn.microsoft.com/en-us/mem/configmgr/develop/apps/how-to-create-the-windows-installer-file-msi

See extension *Microsoft Visual Studio Installer Projects 2022* at: https://marketplace.visualstudio.com/items?itemName=VisualStudioClient.MicrosoftVisualStudio2022InstallerProjects
for more details.

.Net C# -code can be successfully built and debugged without Installer Projects 2022 -extension.
You just cannot create a `.msi`-package.

# WMI

Generate with `mgmtclassgen.exe`:
* mgmtclassgen.exe RegistryEvent /N root\cimv2 /O GPOCover.RegistryUtils /L CS /P .\generated\RegistryEvent.cs
* mgmtclassgen.exe RegistryKeyChangeEvent /N root\cimv2 /O GPOCover.RegistryUtils /L CS /P .\generated\RegistryKeyChangeEvent.cs
* mgmtclassgen.exe RegistryTreeChangeEvent /N root\cimv2 /O GPOCover.RegistryUtils /L CS /P .\generated\RegistryTreeChangeEvent.cs
* mgmtclassgen.exe RegistryValueChangeEvent /N root\cimv2 /O GPOCover.RegistryUtils /L CS /P .\generated\RegistryValueChangeEvent.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectFactories;


namespace GPOCover.Cover.Configuration;

internal static class CoverConfigurationReader
{
    internal static List<CoverConfiguration> Read(DirectoryInfo dirInfo, ILogger logger)
    {
        logger.LogInformation($"Reading configuration YAML-files from: {dirInfo.FullName}");
        var configsOut = new List<CoverConfiguration>();
        var configFilesIn = dirInfo.GetFiles().Where(f => f.Extension == ".yaml").ToList();
        if (!configFilesIn.Any())
        {
            logger.LogError($"There are no GPO Cover configuration YAML-files in {dirInfo.FullName}! Cannot continue.");

            throw new ArgumentException($"There are no GPO Cover configuration YAML-files in {dirInfo.FullName}! Cannot continue.");
        }

        foreach (var configFile in configFilesIn)
        {
            var config = ReadOne(configFile, logger);
            if (string.IsNullOrEmpty(config.Name))
                throw new ArgumentException($"Need trigger name! Mandatory argument.");
            if (config.Trigger is null)
                throw new ArgumentException($"Need trigger definition! Mandatory argument.");
            switch (config.Trigger.Type)
            {
                case Trigger.None:
                case Trigger.Unknown:
                    throw new ArgumentException($"Trigger type {Enum.GetName(config.Trigger.Type)} unsupported!");
                case Trigger.RegistryChange:
                case Trigger.FilesystemChange:
                    if (config.Actions is null || config.Actions.Count == 0)
                        throw new ArgumentException($"Need trigger actions! At least one is needed.");
                    break;
                case Trigger.FilesystemLock:
                default:
                    break;
            }
            configsOut.Add(config);
        }

        return configsOut;
    }

    private static CoverConfiguration ReadOne(FileInfo configurationFile, ILogger logger)
    {

        using (var reader = new StreamReader(configurationFile.FullName))
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            try
            {
                var obj = deserializer.Deserialize<CoverConfiguration>(reader);

                return obj;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to parse YAML-file: {configurationFile.FullName}. Error: {ex.Message}");

                throw;
            }
        }
    }

} // end class CoverConfigurationReader

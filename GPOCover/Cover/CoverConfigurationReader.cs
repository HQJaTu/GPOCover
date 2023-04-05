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


namespace GPOCover.Cover;

internal static class CoverConfigurationReader
{
    internal static List<CoverConfiguration> Read(DirectoryInfo dirInfo, ILogger logger)
    {
        var configsOut = new List<CoverConfiguration>();
        var configFilesIn = dirInfo.GetFiles();
        if (!configFilesIn.Any())
        {
            logger.LogError($"There are no GPO Cover configuration YAML-files in {dirInfo.FullName}! Cannot continue.");

            throw new ArgumentException($"There are no GPO Cover configuration YAML-files in {dirInfo.FullName}! Cannot continue.");
        }

        foreach (var configFile in configFilesIn)
        {
            var config = ReadOne(configFile, logger);
            if (String.IsNullOrEmpty(config.Name))
                throw new ArgumentException($"Need trigger name! Mandatory argument.");
            if (config.Actions is null || config.Actions.Count == 0)
                throw new ArgumentException($"Need trigger actions! At least one is needed.");
            configsOut.Add(config);
        }

        return configsOut;
    }

    private static CoverConfiguration ReadOne(FileInfo configurationFile, ILogger logger)
    {

        using (var reader = new StreamReader(configurationFile.FullName))
        {
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            try
            {
                var obj = deserializer.Deserialize<CoverConfiguration>(reader);

                return obj;
            } catch (Exception ex)
            {
                logger.LogError($"Failed to parse YAML-file: {configurationFile.FullName}. Error: {ex.Message}");

                throw;
            }
        }
    }

} // end class CoverConfigurationReader

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Management;
using System.Text;

using Microsoft.Win32;
using YamlDotNet.Core.Tokens;


namespace GPOCover.RegistryUtils;

public abstract class RegistryChangeBase : ManagementEventWatcher
{
    #region Static Fields

    static List<string> validHiveList = new List<string>() { "HKEY_LOCAL_MACHINE", "HKEY_USERS", "HKEY_CURRENT_CONFIG" };
    static string Format = "{{0}} = '{0}' OR ";

    #endregion Static Fields

    protected readonly ILogger<RegistryChangeBase> _logger;

    #region Constructors

    protected RegistryChangeBase(string Hive, List<string> KeyPathCollection, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RegistryChangeBase>();

        ValidateHive(Hive);

        ValidateKeyPathList(KeyPathCollection);

        this.Query.QueryLanguage = "WQL";
        this.Scope.Path.NamespacePath = @"root\default";
    }

    protected RegistryChangeBase(string Hive, string KeyPath, ILoggerFactory loggerFactory)
        : this(Hive, new List<string>(new string[] { KeyPath }), loggerFactory)
    {
    }

    #endregion Constructors

    #region Private Methods

    private void ValidateHive(string Hive)
    {
        if (!validHiveList.Contains(Hive.ToUpper()))
        {
            if (string.IsNullOrEmpty(Hive))
                throw new ArgumentNullException("Hive", "Hive cannot be null");
            else
                throw new ArgumentException("Hive", "Incorrect value");
        }
    }

    private void ValidateKeyPathList(List<string> KeyPathCollection)
    {
        if (KeyPathCollection.Count == 0)
        {
            throw new ArgumentNullException("Keypath", "KeyPath cannot be null");
        }

        foreach (string item in KeyPathCollection)
        {
            if (string.IsNullOrEmpty(item))
            {
                throw new ArgumentNullException("Keypath", "KeyPath cannot be null");
            }
        }
    }


    #endregion Private Methods

    #region Protected Methods

    protected static string BuildOrString(List<string> Values)
    {
        StringBuilder builder = new StringBuilder();
        foreach (string item in Values)
        {
            builder.Append(string.Format(Format, item));
        }
        builder.Remove(builder.Length - 4, 4);
        return builder.ToString();
    }


    #endregion Protected Methods
}
﻿using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace GPOCover.RegistryUtils;
#pragma warning disable 1416

public class RegistryValueChange : RegistryChangeBase
{
    #region Constants

    //private const string queryString = "SELECT * FROM RegistryValueChangeEvent WHERE Hive = '{0}' AND KeyPath = '{1}' AND ValueName = '{2}'";
    private const string queryString = "SELECT * FROM RegistryValueChangeEvent WHERE Hive = '{0}' AND KeyPath = '{1}' AND ({2})";

    #endregion Constants

    #region Static Fields

    private static string HiveLocation = "ValueName";

    #endregion Static Fields

    #region Constructors

    public RegistryValueChange(string Hive, string KeyPath, string ValueName, ILoggerFactory loggerFactory)
        : this(Hive, KeyPath, new List<string>(new string[] { ValueName }), loggerFactory)
    { }

    public RegistryValueChange(RegistryKey Hive, string KeyPath, string ValueName, ILoggerFactory loggerFactory)
        : this(Hive.Name, KeyPath, ValueName, loggerFactory)
    {
    }

    public RegistryValueChange(string Hive, string KeyPath, List<string> ValueNameCollection, ILoggerFactory loggerFactory)
        : base(Hive, KeyPath, loggerFactory)
    {
        foreach (string item in ValueNameCollection)
        {
            if (string.IsNullOrEmpty(item))
            {
                throw new ArgumentNullException("ValueName", "ValueName cannot be null");
            }
        }

        this.Query.QueryString = BuildQueryString(Hive, KeyPath, ValueNameCollection);
        //string.Format(queryString, Hive, KeyPath, ValueName);

        this.EventArrived += new EventArrivedEventHandler(RegistryValueChange_EventArrived);
    }

    public RegistryValueChange(RegistryKey Hive, string KeyPath, List<string> ValueNameCollection, ILoggerFactory loggerFactory)
        : this(Hive.Name, KeyPath, ValueNameCollection, loggerFactory)
    {
    }

    #endregion Constructors

    #region Private Methods

    private string BuildQueryString(string Hive, string KeyPath, List<string> ValueNameCollection)
    {
        string ORString = RegistryChangeBase.BuildOrString(ValueNameCollection);
        string FormattedOR = string.Format(ORString, HiveLocation);
        return string.Format(queryString, Hive, KeyPath, FormattedOR);
    }

    private void RegistryValueChange_EventArrived(object sender, EventArrivedEventArgs e)
    {
        RegistryValueChangeEvent RegValueChange = new RegistryValueChangeEvent(e.NewEvent);

        OnRegistryValueChanged(RegValueChange);
    }

    protected virtual void OnRegistryValueChanged(RegistryValueChangeEvent RegValueChange)
    {
        if (RegistryValueChanged != null)
        {
            RegistryValueChanged(this, new RegistryValueChangedEventArgs(RegValueChange));
        }
    }

    #endregion Private Methods

    #region Events

    public event EventHandler<RegistryValueChangedEventArgs>? RegistryValueChanged = null;

    #endregion Events
}

public class RegistryValueChangedEventArgs : EventArgs
{
    private RegistryValueChangeEvent data;

    public RegistryValueChangeEvent RegistryValueChangeData
    {
        get
        {
            return data;
        }
    }

    public RegistryValueChangedEventArgs(RegistryValueChangeEvent Data)
    {
        data = Data;
    }
}
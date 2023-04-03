using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using Microsoft.Win32;

namespace GPOCover.RegistryUtils;
#pragma warning disable 1416

public class RegistryKeyChange : RegistryChangeBase
{
    #region Constants

    private const string queryString = "SELECT * FROM RegistryKeyChangeEvent WHERE Hive = '{0}' AND ({1})";

    #endregion Constants

    #region Static Fields

    private static string HiveLocation = "KeyPath";

    #endregion Static Fields

    #region Constructors

    public RegistryKeyChange(string Hive, string KeyPath, ILoggerFactory loggerFactory)
        : this(Hive, new List<string>(new string[] { KeyPath }), loggerFactory)
    {
    }

    public RegistryKeyChange(RegistryKey Hive, string KeyPath, ILoggerFactory loggerFactory)
        : this(Hive.Name, KeyPath, loggerFactory)
    {
    }

    public RegistryKeyChange(string Hive, List<string> KeyPathCollection, ILoggerFactory loggerFactory)
        : base(Hive, KeyPathCollection, loggerFactory)
    {
        this.Query.QueryString = BuildQueryString(Hive, KeyPathCollection);

        this.EventArrived += new EventArrivedEventHandler(RegistryKeyChange_EventArrived);
    }

    public RegistryKeyChange(RegistryKey Hive, List<string> KeyPathCollection, ILoggerFactory loggerFactory)
        : this(Hive.Name, KeyPathCollection, loggerFactory)
    {
    }

    #endregion Constructors

    #region Private Methods

    private string BuildQueryString(string Hive, List<string> KeyPathCollection)
    {
        string ORString = RegistryChangeBase.BuildOrString(KeyPathCollection);
        string FormattedOR = string.Format(ORString, HiveLocation);
        var query = string.Format(queryString, Hive, FormattedOR);
        this._logger.LogDebug($"{this.GetType().Name}: BuildQueryString() = '{query}'");

        return query;
    }

    private void RegistryKeyChange_EventArrived(object sender, EventArrivedEventArgs e)
    {
        RegistryKeyChangeEvent RegValueChange = new RegistryKeyChangeEvent(e.NewEvent);

        OnRegistryKeyChanged(RegValueChange);
    }

    protected virtual void OnRegistryKeyChanged(RegistryKeyChangeEvent RegValueChange)
    {
        if (RegistryKeyChanged != null)
        {
            RegistryKeyChanged(this, new RegistryKeyChangedEventArgs(RegValueChange));
        }
    }

    #endregion Private Methods

    #region Events

    public event EventHandler<RegistryKeyChangedEventArgs>? RegistryKeyChanged = null;

    #endregion Events
}

public class RegistryKeyChangedEventArgs : EventArgs
{
    private RegistryKeyChangeEvent data;

    public RegistryKeyChangeEvent RegistryKeyChangeData
    {
        get
        {
            return data;
        }
    }

    public RegistryKeyChangedEventArgs(RegistryKeyChangeEvent Data)
    {
        data = Data;
    }
}
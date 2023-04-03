using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using Microsoft.Win32;

namespace GPOCover.RegistryUtils;
#pragma warning disable 1416

public class RegistryTreeChange : RegistryChangeBase
{
    #region Constants

    private const string queryString = "SELECT * FROM RegistryTreeChangeEvent WHERE Hive = '{0}' AND ({1})";

    #endregion Constants

    #region Static Fields

    private static string HiveLocation = "RootPath";

    #endregion Static Fields

    #region Constructors

    public RegistryTreeChange(string Hive, string RootPath, ILoggerFactory loggerFactory)
        : this(Hive, new List<string>(new string[] { RootPath }), loggerFactory)
    { }

    public RegistryTreeChange(RegistryKey Hive, string RootPath, ILoggerFactory loggerFactory)
        : this(Hive.Name, RootPath, loggerFactory)
    {
    }

    public RegistryTreeChange(string Hive, List<string> RootPathCollection, ILoggerFactory loggerFactory)
        : base(Hive, RootPathCollection, loggerFactory)
    {
        this.Query.QueryString = BuildQueryString(Hive, RootPathCollection);

        this.EventArrived += new EventArrivedEventHandler(RegistryTreeChange_EventArrived);
    }

    public RegistryTreeChange(RegistryKey Hive, List<string> RootPathCollection, ILoggerFactory loggerFactory)
        : this(Hive.Name, RootPathCollection, loggerFactory)
    {
    }

    #endregion Constructors

    #region Private Methods

    private string BuildQueryString(string Hive, List<string> KeyPathCollection)
    {
        string ORString = RegistryChangeBase.BuildOrString(KeyPathCollection);
        string FormattedOR = string.Format(ORString, HiveLocation);
        return string.Format(queryString, Hive, FormattedOR);
    }

    private void RegistryTreeChange_EventArrived(object sender, EventArrivedEventArgs e)
    {
        RegistryTreeChangeEvent RegTreeChange = new RegistryTreeChangeEvent(e.NewEvent);

        OnRegistryTreeChanged(RegTreeChange);
    }

    protected virtual void OnRegistryTreeChanged(RegistryTreeChangeEvent RegTreeChange)
    {
        if (RegistryTreeChanged != null)
        {
            RegistryTreeChanged(this, new RegistryTreeChangedEventArgs(RegTreeChange));
        }
    }

    #endregion Private Methods

    #region Events

    public event EventHandler<RegistryTreeChangedEventArgs>? RegistryTreeChanged = null;

    #endregion Events
}

public class RegistryTreeChangedEventArgs : EventArgs
{
    private RegistryTreeChangeEvent data;

    public RegistryTreeChangeEvent RegistryTreeChangeData
    {
        get
        {
            return data;
        }
    }

    public RegistryTreeChangedEventArgs(RegistryTreeChangeEvent Data)
    {
        data = Data;
    }
}
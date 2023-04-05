using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPOCover.Cover.Actions;
using Microsoft.Extensions.Options;

namespace GPOCover.Cover.Triggers;

internal class TriggerBase
{
    public uint Id { get; }
    internal List<ActionBase> _actions;

    public TriggerBase(uint id)
    {
        this.Id = id;
        this._actions = new List<ActionBase>();
    }

    public void AddActions(List<ActionBase> actions)
    {
        foreach (var action in actions)
            this._actions.Add(action);
    }

    public void AddAction(ActionBase action)
    {
        this._actions.Add(action);
    }

    internal void RunActions()
    {
        foreach (var action in this._actions)
        {
            action.RunAsync().GetAwaiter();
        }
    }

} // end class TriggerBase

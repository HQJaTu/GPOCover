using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPOCover.Cover.Actions;
using Microsoft.Extensions.Options;

namespace GPOCover.Cover.Triggers;

internal abstract class TriggerBase
{
    public uint Id { get; }
    internal List<ActionBase> _actions;
    internal bool _runningActions = false;

    public TriggerBase(uint id)
    {
        this.Id = id;
        this._actions = new List<ActionBase>();
        this._runningActions = false;
    }

    public abstract void Start();

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
        if (this._runningActions)
            return;

        // Go run something!
        // Do it asynchronously from this synchronous function.
        // We will not block for duration of DoRunActions(). This function will exit almost instantly.
        this._runningActions = true;

        DoRunActions().GetAwaiter().OnCompleted(() => {
            this._runningActions = false;
        });
    }

    internal async Task DoRunActions()
    {
        foreach (var action in this._actions)
        {
            await action.RunAsync();
        }
    }

} // end class TriggerBase

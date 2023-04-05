using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPOCover.Cover.Actions;

internal abstract class ActionBase
{
    abstract public Task RunAsync();

} // end class ActionBase

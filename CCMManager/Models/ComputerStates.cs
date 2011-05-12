using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCMManager.Models
{
    [Flags]
    public enum ComputerStates
    {
        Online,
        Offline,
        LoggedOn,
        Unknown,
        AccessDenied
    }
}

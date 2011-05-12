

namespace CCMManager.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Caliburn.Micro;

    public interface IChildScreen : IScreen
    {
        string ScreenId { get; }
    }

    public interface IChildScreen<TParent> : IChildScreen
        where TParent : IConductor
    {
        int? Order { get; }
    }
}

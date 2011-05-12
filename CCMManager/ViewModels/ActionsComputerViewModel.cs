using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCMManager.ViewModels
{
    using System.ComponentModel.Composition;
    using Caliburn.Micro;
    using Framework;

    /// <summary>
    /// Defines the content of a single TabItem to display information about a single Computer.
    /// </summary>
    [Export(typeof(ActionsComputerViewModel))]
    [Export(typeof(IChildScreen<ActionsViewModel>))]
    public class ActionsComputerViewModel : Screen, IChildScreen<ActionsViewModel>
    {
        public string ScreenId
        {
            get { return GetType().Name; }
        }

        public int? Order
        {
            get { return 1; }
        }
    }
}

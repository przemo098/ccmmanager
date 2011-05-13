using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCMManager.ViewModels
{
    using Caliburn.Micro;
    using Framework;
    using Models;
    using System.ComponentModel.Composition;

    [Export(typeof(ComputerDetailsViewModel))]
    [Export(typeof(IChildScreen<ActionsViewModel>))]
    public class ComputerDetailsViewModel : Screen, IChildScreen<ActionsViewModel>
    {
        [ImportingConstructor]
        public ComputerDetailsViewModel()
        {

        }

        public int? Order
        {
            get { return 1; }
        }

        public string ScreenId
        {
            get { return GetType().Name; }
        }
    }
}

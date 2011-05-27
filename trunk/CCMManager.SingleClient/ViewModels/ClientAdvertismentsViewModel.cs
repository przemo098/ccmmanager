using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.ComponentModel.Composition;

namespace CCMManager.SingleClient.ViewModels
{
    [Export(typeof(ClientAdvertismentsViewModel))]
    public class ClientAdvertismentsViewModel : Conductor<IScreen>.Collection.OneActive
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace CCMManager.SingleClient.ViewModels
   
{
    [Export(typeof(ClientAgentViewModel))]
    public class ClientAgentViewModel : Conductor<IScreen>.Collection.OneActive
    {

    }
}

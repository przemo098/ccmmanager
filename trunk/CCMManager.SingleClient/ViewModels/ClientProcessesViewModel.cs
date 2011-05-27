using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace CCMManager.SingleClient.ViewModels
{
    [Export(typeof(ClientProcessesViewModel))]
    public class ClientProcessesViewModel : Conductor<IScreen>.Collection.OneActive
    {

    }
}

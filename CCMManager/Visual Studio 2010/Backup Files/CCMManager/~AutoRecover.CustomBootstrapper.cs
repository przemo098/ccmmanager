using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Caliburn.Micro;

namespace CCMManager
{
    using CCMManager.ViewModels;
    using CCMManager.Framework;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    

    public class CustomBootstrapper : Bootstrapper<ShellViewModel>
    {
        private CompositionContainer container;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCMManager.ViewModels
{
    using System.ComponentModel.Composition;
    using Caliburn.Micro;
    using Framework;

    [Export(typeof(ActionsHomeViewModel))]
    [Export(typeof(IChildScreen<ActionsViewModel>))]
    public class ActionsHomeViewModel : Screen, IChildScreen<ActionsViewModel>
    {

        [ImportingConstructor]
        public ActionsHomeViewModel()
        {
            DisplayName = "Actions";
        }

        public string ScreenId
        {
            get { return GetType().Name; }
        }

        public int? Order 
        { 
            get { return 0; } 
        }

        public void EditClassrooms(object o)
        {
            //Load the EditorViewModel instead of this one...
            var myDirectParent = Parent as ActionsViewModel;
            var realParent = myDirectParent.Parent as ShellViewModel;

            realParent.ActivateItem(new EditorViewModel());
            
        }
    }
}

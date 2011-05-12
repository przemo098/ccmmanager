using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCMManager.ViewModels
{
    using Caliburn.Micro;
    using System.ComponentModel.Composition;
    using Framework;

    /// <summary>
    /// Defines the base itemscontrol and maintains an ActiveItem to hold either:
    /// a single instance of ActionsHomeViewModel, or multiple instances of ActionsComputerViewModel
    /// </summary>
    
    [Export(typeof(ActionsViewModel))]
    public class ActionsViewModel : Conductor<IScreen>.Collection.OneActive
    {
        [ImportingConstructor]
        public ActionsViewModel([ImportMany(AllowRecomposition = true)] IEnumerable<IChildScreen<ActionsViewModel>> childScreens)
        {
            DisplayName = "Actions Display";
            FillItems(childScreens);
        }

        internal void FillItems<TParent>(IEnumerable<IChildScreen<TParent>> childScreens)
            where TParent : IConductor
        {
            if (childScreens == null || childScreens.Count() == 0)
                return;

            var homeScreens = childScreens.Where(
                x => x.GetType().Name.ToString().Contains("HomeViewModel")).OrderBy(x => x.Order);

            foreach (var home in homeScreens)
            {
                if (home.Order != null && home.Order < Items.Count)
                    Items.Insert((int)home.Order, home);
                else
                    Items.Add(home);
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (Items.Count > 0)
                ActivateItem(Items.FirstOrDefault());
        }

        public override void ActivateItem(IScreen item)
        {
            //base.ActivateItem(CheckIfScreenExists(item));
            base.ActivateItem(item);
        }

        public void EditClassrooms(object o)
        {
            ActivateItem(Items[1]);
        }
    }
}

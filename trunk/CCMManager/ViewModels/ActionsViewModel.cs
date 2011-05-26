//CCMManager
//Copyright (c) 2011 by David Kamphuis
//
//   This file is part of CCMManager.
//
//    CCMManager is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Foobar is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using CCMManager.Framework;

namespace CCMManager.ViewModels
{
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

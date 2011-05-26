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

using System.ComponentModel.Composition;
using Caliburn.Micro;
using CCMManager.Framework;

namespace CCMManager.ViewModels
{
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

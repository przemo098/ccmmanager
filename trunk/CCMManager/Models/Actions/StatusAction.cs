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

using System;
using System.Collections.Generic;
using System.Windows.Media;
using CCMManager.Services;

namespace CCMManager.Models.Actions
{
    public class StatusAction : RemoteStatusAction
    {
        public StatusAction(Dictionary<ComputerStates, ImageSource> imgs = null)
            :base(imgs)
        {

        }
        
        public void Execute(ActionsHomeModel context = null)
        {
            //ComputerStates state = new ComputerStates();
            ActionsHomeModel client = context;
            try
            {
                WMIProvider oWMI = new WMIProvider(client.Name);
                this.State = oWMI.GetHostStatus();
                
                //Update the GUI...
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        client.Status = this.State;
                    }), null);

                if (this.State == ComputerStates.Online | this.State == ComputerStates.LoggedOn)
                {
                    client.MacAddresses = oWMI.RetreiveMacAddresses();
                    if (this.State == ComputerStates.LoggedOn)
                    {
                        string user = oWMI.ReturnUserName();
                        App.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                client.LoggedOnUser = user;
                            }), null);
                    }
                }
            }
            finally
            {
                GC.Collect();
            }
            //Do the Task Here...
        }
    }
}

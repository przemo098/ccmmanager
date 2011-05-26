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
using System.Management;
using System.Windows.Media;

namespace CCMManager.Models.Actions
{
    public class ShutdownAction : RemoteAction
    {
        ManagementPath oRootPath;
        ManagementScope oRootMs;
        ConnectionOptions oCon = new ConnectionOptions();

        public ShutdownAction(Dictionary<RemoteActionState, ImageSource> images)
            :base(images)
        {
            this.RemoteActionName = "Initiate Forced Shutdown";
        }
        
        public override void Execute(object context = null)
        {
            ActionsHomeModel pc = context as ActionsHomeModel;
            //Can only be processed if the machine is online...
            if (pc.Status == ComputerStates.Online || pc.Status == ComputerStates.LoggedOn)
            {
                //Perform the required action...
                oRootPath = new ManagementPath(string.Format("\\\\{0}\\ROOT\\CIMV2", pc.Name));
                oCon.Impersonation = ImpersonationLevel.Impersonate;
                oCon.EnablePrivileges = true;
                oRootMs = new ManagementScope(oRootPath, oCon);
                ManagementBaseObject inParams, outParams;
                int result;

                try
                {
                    oRootMs.Connect();
                    ObjectQuery oQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                    ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oRootMs, oQuery);
                    foreach (ManagementObject mo in oSearcher.Get())
                    {
                        inParams = mo.GetMethodParameters("Win32Shutdown");
                        inParams["Flags"] = 5; //ForcedShutdown; -- fixed to shutdown
                        inParams["Reserved"] = 0;

                        outParams = mo.InvokeMethod("Win32Shutdown", inParams, null);
                        result = Convert.ToInt32(outParams["returnValue"]);

                        if (result != 0)
                        {
                            App.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.State = RemoteActionState.Error;
                            }), null);
                        }
                        else
                        {
                            App.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.State = RemoteActionState.Completed;
                            }), null);
                        }

                    }
                    oSearcher.Dispose();

                }
                catch
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.State = RemoteActionState.Error;
                    }), null);
                }
                finally
                {
                }
                
            }
            else
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.State = RemoteActionState.Pending;
                }), null);
            }
        }
    }
}

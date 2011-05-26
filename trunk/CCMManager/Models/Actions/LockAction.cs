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
    public class LockAction : RemoteAction
    {
        ManagementPath oRootPath;
        ManagementScope oRootMs;
        ConnectionOptions oCon = new ConnectionOptions();
        object[] theProcessToRun = { "rundll32.exe user32.dll,LockWorkStation" };

        public LockAction(Dictionary<RemoteActionState, ImageSource> images)
            :base(images)
        {
        }

        public override void Execute(object context = null)
        {
            ActionsHomeModel pc = context as ActionsHomeModel;
            //Can only be processed if the machine is logged on...
            if (pc.Status == ComputerStates.LoggedOn)
            {
                //Perform the required action...
                oRootPath = new ManagementPath(string.Format("\\\\{0}\\ROOT\\CIMV2", pc.Name));
                oCon.Impersonation = ImpersonationLevel.Impersonate;
                oCon.EnablePrivileges = true;
                oRootMs = new ManagementScope(oRootPath, oCon);

                try
                {
                    oRootMs.Connect();
                    ObjectGetOptions oGet = new ObjectGetOptions();
                    ManagementClass oClass = new ManagementClass(oRootMs, new ManagementPath("Win32_ScheduledJob"), new ObjectGetOptions());
                    ManagementBaseObject inParams = oClass.GetMethodParameters("Create");
                    inParams["StartTime"] = ManagementDateTimeConverter.ToDmtfDateTime(DateTime.Now.AddMinutes(1));
                    inParams["InteractWithDesktop"] = 1;
                    //inParams["Command"] = "rundll32 user32.dll,LockWorkStation";
                    inParams["Command"] = "notepad.exe";
                    ManagementBaseObject outParams = oClass.InvokeMethod("Create", inParams, null);


                    Console.WriteLine("Result was: {0} - JobId: {1}", outParams["ReturnValue"], outParams["JobId"]);

                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.State = RemoteActionState.Completed;
                    }), null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.State = RemoteActionState.Error;
                    }), null);
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

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
    public class PolicyHardResetAction : RemoteAction
    {
        public PolicyHardResetAction(Dictionary<RemoteActionState, ImageSource> images)
            : base(images)
        {
            this.RemoteActionName = "Initate Policy Hard Reset";
        }

        public override void Execute(object context = null)
        {
            ActionsHomeModel pc = context as ActionsHomeModel;
            //Can only be processed if the machine is online...
            if (pc.Status == ComputerStates.Online || pc.Status == ComputerStates.LoggedOn)
            {
                ManagementScope oMs = ConnectToClient(pc.Name);
                if (oMs != null)
                {
                    this.DeleteQueryData(oMs, @"root\ccm\Policy", "SELECT * FROM CCM_SoftwareDistribution");
                    this.DeleteQueryData(oMs, @"root\ccm\Policy", "SELECT * FROM CCM_Scheduler_ScheduledMessage");
                    this.DeleteQueryData(oMs, @"root\ccm\Scheduler", "SELECT * FROM CCM_Scheduler_History");

                    //oMs.Path.NamespacePath = @"ROOT\CCM";
                    ManagementClass oClass = new ManagementClass(oMs, new ManagementPath("SMS_Client"), null);
                    oClass.Get();
                    ManagementBaseObject inParams = oClass.GetMethodParameters("ResetPolicy");
                    inParams["uFlags"] = 1;
                    oClass.InvokeMethod("ResetPolicy", inParams, new InvokeMethodOptions());

                    ManagementBaseObject newParams = oClass.GetMethodParameters("RequestMachinePolicy");
                    oClass.InvokeMethod("RequestMachinePolicy", newParams, new InvokeMethodOptions());

                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.State = RemoteActionState.Completed;
                    }), null);
                }
                else
                {
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

        private ManagementScope ConnectToClient(string hostname)
        {
            ManagementPath oPath = new ManagementPath(string.Format("\\\\{0}\\ROOT\\CCM", hostname));
            ConnectionOptions oCon = new ConnectionOptions();
            oCon.Impersonation = ImpersonationLevel.Impersonate;
            oCon.EnablePrivileges = true;
            ManagementScope oMs = new ManagementScope(oPath, oCon);
            try
            {
                oMs.Connect();
            }
            catch (System.UnauthorizedAccessException ex)
            {
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
            return oMs;
        }

        private bool RunScheduleID(string triggerID, ManagementScope oMs)
        {
            try
            {
                ManagementClass cls = new ManagementClass(oMs.Path.Path, "SMS_Client", null);
                ManagementBaseObject inParams, outMPParams;
                inParams = cls.GetMethodParameters("TriggerSchedule");
                inParams["sScheduleID"] = triggerID;
                outMPParams = cls.InvokeMethod("TriggerSchedule", inParams, null);
                cls.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void DeleteQueryData(ManagementScope oScope, string sNamespace, string sQuery)
        {
            try
            {
                ManagementScope oMs = new ManagementScope(oScope.Path.Clone());
                oMs.Path.NamespacePath = sNamespace;
                ManagementObjectCollection moc = new ManagementObjectSearcher(oMs, new ObjectQuery(sQuery)).Get();
                foreach (ManagementObject mo in moc)
                {
                    mo.Delete();
                }
   
            }
            catch { }
        }
    }
}

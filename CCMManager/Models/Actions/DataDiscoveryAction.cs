using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Windows.Media;

namespace CCMManager.Models.Actions
{
    class DataDiscoveryAction : RemoteAction
    {
        public DataDiscoveryAction(Dictionary<RemoteActionState, ImageSource> images)
            : base(images)
        {

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
                    if (RunScheduleID("{00000000-0000-0000-0000-000000000003}", oMs)) //Run DDR.
                    {
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
                        this.State = RemoteActionState.Error;
                    }), null);
                }
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
    }
}

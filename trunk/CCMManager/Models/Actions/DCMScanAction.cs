using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Management;

namespace CCMManager.Models.Actions
{
    public class DCMScanAction : RemoteAction
    {
         public DCMScanAction(Dictionary<RemoteActionState, ImageSource> images)
            :base(images)
        {
        }

        public override void Execute(object context = null)
        {
            ActionsHomeModel pc = context as ActionsHomeModel;
            //Can only be processed if the machine is online...
            if (pc.Status == ComputerStates.Online || pc.Status == ComputerStates.LoggedOn)
            {
                ManagementScope oMs = ConnectToClient(pc.Name);
                string sDCMScheduleID = null;
                if (oMs != null && string.IsNullOrEmpty(sDCMScheduleID))
                {
                    ManagementScope oMs2 = new ManagementScope(string.Format("\\\\{0}\\ROOT\\ccm\\policy\\machine\\actualconfig", pc.Name));
                    ManagementObjectCollection moc = new ManagementObjectSearcher(oMs2, new ObjectQuery("Select * from CCM_Scheduler_ScheduledMessage WHERE TargetEndPoint = 'direct:DCMAgent'")).Get();
                    foreach (ManagementObject mo in moc)
                    {
                        try
                        {
                            string sID = mo["ScheduledMessageID"].ToString();
                            if (!string.IsNullOrEmpty(sID))
                            {
                                sDCMScheduleID = sID;
                                if (string.IsNullOrEmpty(sDCMScheduleID))
                                {
                                    //throw new System.Exception("Not Found!");
                                }
                                else
                                {
                                    if (RunScheduleID(sDCMScheduleID, oMs))
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
                            }
                        }
                        catch
                        {
                            App.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                this.State = RemoteActionState.Error;
                            }), null);
                        }
                    }
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

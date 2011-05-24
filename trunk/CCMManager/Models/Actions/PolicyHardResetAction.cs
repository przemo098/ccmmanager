using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Management;

namespace CCMManager.Models.Actions
{
    public class PolicyHardResetAction : RemoteAction
    {
        public PolicyHardResetAction(Dictionary<RemoteActionState, ImageSource> images)
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
                    oClass.InvokeMethod("RequestMachinePolicy",newParams, new InvokeMethodOptions() );

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

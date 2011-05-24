using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Management;

namespace CCMManager.Models.Actions
{
    class PolicyResetAction : RemoteAction
    {
        public PolicyResetAction(Dictionary<RemoteActionState, ImageSource> images)
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
                    try
                    {
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
                    catch (Exception e)
                    {
                        App.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.State = RemoteActionState.Error;
                        }), null);
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Management.Instrumentation;
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
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Management;

namespace CCMManager.Models.Actions
{
    public class GPUpdateAction : RemoteAction
    {

        ManagementPath oRootPath;
        ManagementScope oRootMs;
        ConnectionOptions oCon = new ConnectionOptions();

        public GPUpdateAction(Dictionary<RemoteActionState, ImageSource> images)
            :base(images)
        {
        }

        public override void Execute(object context = null)
        {
            ActionsHomeModel pc = context as ActionsHomeModel;
            //Can only be processed if the machine is turned on.
            if (pc.Status == ComputerStates.LoggedOn || pc.Status == ComputerStates.Online)
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
                    ManagementClass oClass = new ManagementClass(oRootMs, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                    ManagementBaseObject inParams = oClass.GetMethodParameters("Create");
                    inParams["CommandLine"] = @"gpupdate /Force";
                    ManagementBaseObject outParams = oClass.InvokeMethod("Create", inParams, null);


                    //Console.WriteLine("Result was: {0} - JobId: {1}", outParams["ReturnValue"], outParams["ProcessId"]);

                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.State = RemoteActionState.Completed;
                    }), null);
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);

                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.State = RemoteActionState.Error;
                    }), null);
                }
            }
        }
    }
}

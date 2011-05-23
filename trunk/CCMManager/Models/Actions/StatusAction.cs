namespace CCMManager.Models.Actions
{
    using System.Windows.Media;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Services;

    public class StatusAction : RemoteStatusAction
    {
        public StatusAction(Dictionary<ComputerStates, ImageSource> imgs)
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
                //dispose of the wmiobjects.
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                client.CurrentStatusImage = this.CurrentStatusImage;
                            }), null);
            }
            //Do the Task Here...
        }
    }
}

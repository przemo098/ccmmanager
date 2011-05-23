using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Net.Sockets;
using System.Net;

namespace CCMManager.Models.Actions
{
    public class WakeOnLanAction : RemoteAction
    {
        UdpClient client = new UdpClient();
        Byte[] datagram = new byte[102];
        string[] macDigits = null;

        public WakeOnLanAction(Dictionary<RemoteActionState, ImageSource> images)
            :base(images)
        {
            for (int i = 0; i <= 5; i++)
            {
                datagram[i] = 0xff;
            }
        }

        public override void Execute(object context = null)
        {
            ActionsHomeModel pc = context as ActionsHomeModel;
            try
            {
                if (pc.MacAddresses.Count != 0)
                {
                    foreach (string macAddress in pc.MacAddresses)
                    {
                        //send a wol to each...
                        if (macAddress.Contains("-"))
                        {
                            macDigits = macAddress.Split('-');
                        }
                        else
                        {
                            macDigits = macAddress.Split(':');
                        }
                        if (macDigits.Length != 6)
                        {
                            //Error
                        }

                        int start = 6;
                        for (int i = 0; i < 16; i++)
                        {
                            for (int x = 0; x < 6; x++)
                            {
                                datagram[start + i * 6 + x] = (byte)Convert.ToInt32(macDigits[x], 16);
                            }
                        }

                        client.Connect(IPAddress.Broadcast, 7);
                        client.Send(datagram, datagram.Length);
                    }
                }
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.State = RemoteActionState.Completed;
                }), null);
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

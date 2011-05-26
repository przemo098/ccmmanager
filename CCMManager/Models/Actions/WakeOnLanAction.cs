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
using System.Net;
using System.Net.Sockets;
using System.Windows.Media;

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
            this.RemoteActionName = "Trigger WOL";
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

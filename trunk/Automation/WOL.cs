//CCMManager
//Copyright (c) 2008 by Roger Zander
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

namespace CCMManager.Automation
{
    public class WOL
    {
        #region Private Fields

        List<string> _macAddresses;
        UdpClient client = new UdpClient();
        Byte[] datagram = new byte[102];
        string[] macDigits = null;

        #endregion //Private Fields

        #region Constructors

        /// <summary>
        /// Constructor for Wake On Lan
        /// </summary>
        /// <param name="macAddresses">A list of MacAddresses</param>
        public WOL(List<string> macAddresses)
        {
            _macAddresses = macAddresses;
        }

        /// <summary>
        /// Constructor for WakeOnLan
        /// </summary>
        /// <param name="macAddress"></param>
        public WOL(string macAddress)
        {
            this._macAddresses = new List<string>();
            this._macAddresses.Add(macAddress);
        }

        #endregion //Constructors

        #region Public Methods

        /// <summary>
        /// Sends a ping to the _macAddresses listed.
        /// </summary>
        public void Send()
        {
            try
            {
                if (_macAddresses.Count != 0)
                {
                    foreach (string macAddress in _macAddresses)
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
            }
            catch
            {
            }
        }

        #endregion //Public Methods
    }
}

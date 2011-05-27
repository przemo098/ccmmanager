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
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using CCMManager.Models;
using CCMManager.Automation.WMI;
namespace CCMManager.Services
{
    public class WMIProvider : IDisposable
    {
        ManagementPath oCCMPath;
        ManagementPath oRootPath;
        ConnectionOptions oCon = new ConnectionOptions();
        ManagementScope oRootMs;
        string Hostname = null;
        private string LoggedOnUserName = null;
        private List<string> _macAddresses = new List<string>();
        //private Provider oProv;

        public WMIProvider(string hostname)
        {
            oRootPath = new ManagementPath(string.Format("\\\\{0}\\ROOT\\CIMV2", hostname));
            //oCCMPath = new ManagementPath(string.Format("\\\\{0}\\ROOT\\CCM", hostname));
            oCon.Impersonation = ImpersonationLevel.Impersonate;
            oCon.EnablePrivileges = true;
            oRootMs = new ManagementScope(oRootPath, oCon);
            this.Hostname = hostname;
            //oProv = new Provider(oRootPath.NamespacePath);
            
        }

       
        
        public ComputerStates GetHostStatus()
        {
            try
            {
                ComputerStates state = ComputerStates.Unknown;
                if (PingHost() == true)
                {
                    state = ComputerStates.Online;
                    state = HostAccessOk();
                    if (state == ComputerStates.Online)
                    {
                        if (!HostDNSIsCorrect())
                        {
                            state = ComputerStates.DNSError;
                        }
                        else
                        {
                            //DNS Ok, get userstate...
                            if (HostHasUser())
                            {
                                state = ComputerStates.LoggedOn;
                            }
                            // Get MAC Addresses...
                            this.GetHostMACAddresses();
                        }
                    }
                }
                else
                {
                    state = ComputerStates.Offline;
                }
                return state;
            }
            finally
            {
                
            }
        }
        
        private ComputerStates HostAccessOk()
        {
            try
            {
                oRootMs.Connect();
                return ComputerStates.Online;
            }
            catch (UnauthorizedAccessException ex)
            {
                return ComputerStates.AccessDenied;
            }
            catch (ManagementException ex)
            {
                return ComputerStates.AccessDenied;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                return ComputerStates.Offline;
            }
            catch (Exception ex)
            {
                return ComputerStates.Offline;
            }
        }

        private bool PingHost()
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int timeout = 400;
            try
            {
                PingReply reply = pingSender.Send(this.Hostname, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    pingSender.Dispose();
                    return true;
                }
                else
                {
                    pingSender.Dispose();
                    return false;
                }
            }
            catch
            {
                pingSender.Dispose();
                return false;
            }
        }

        private bool HostDNSIsCorrect()
        {
            //ComputerSystem cs = new ComputerSystem(oProv);
            //ManagementObject mo1 = cs.Win32_ComputerSystem;
            //mo1.GetPropertyValue("DNSHostName").ToString();

            ObjectQuery oQuery = new ObjectQuery("SELECT DNSHostName FROM Win32_ComputerSystem");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oRootMs, oQuery);
            
            try
            {
                //Check that the remote host is the current right one!
                ManagementObjectCollection moc = oSearcher.Get();
                if (moc.Count != 0)
                {
                    foreach (ManagementObject mo in moc)
                    {
                        string result = mo["DNSHostName"].ToString();
                        if (result.ToLower() == Hostname.ToLower())
                        {
                            mo.Dispose();
                            moc.Dispose();
                            return true;
                        }
                        else
                        {
                            mo.Dispose();
                            moc.Dispose();
                            return false;
                        }
                    }
                }
                moc.Dispose();
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                oSearcher.Dispose();
            }
        }

        private bool HostHasUser()
        {
            //Check that the remote host is the current right one!
            ObjectQuery oQuery = new ObjectQuery("SELECT UserName FROM Win32_ComputerSystem");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oRootMs, oQuery);
            try
            {
                ManagementObjectCollection moc = oSearcher.Get();
                if (moc.Count != 0)
                {
                    foreach (ManagementObject mo in moc)
                    {
                        string result = mo["UserName"].ToString();
                        if (result != "")
                        {
                            this.LoggedOnUserName = result;
                            mo.Dispose();
                            return true;
                        }
                    }
                }
                moc.Dispose();
                return false;
            }
            catch
            {
                this.LoggedOnUserName = null;
                return false;
            }
            finally
            {
                oSearcher.Dispose();
            }
        }

        private void GetHostMACAddresses()
        {
            ObjectQuery oQuery = new ObjectQuery("SELECT MACAddress FROM win32_networkadapter WHERE MACAddress != null and NetEnabled = TRUE");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oRootMs, oQuery);
            try
            {
                //Check that the remote host is the current right one!

                ManagementObjectCollection moc = oSearcher.Get();
                if (moc.Count != 0)
                {
                    foreach (ManagementObject mo in moc)
                    {
                        this._macAddresses.Add(mo["MACAddress"].ToString());
                        mo.Dispose();
                    }
                }
                moc.Dispose();
            }
            catch
            {

            }
            finally
            {
                oSearcher.Dispose();
            }
        }

        public string ReturnUserName()
        {
            return this.LoggedOnUserName;
        }

        public List<string> RetreiveMacAddresses()
        {
            return _macAddresses;
        }


        public void Dispose()
        {
            GC.Collect();
        }
    }
}

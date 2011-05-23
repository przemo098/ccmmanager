using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Instrumentation;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;

namespace CCMManager.Services
{
    using Models;

    public class WMIProvider
    {
        ManagementPath oCCMPath;
        ManagementPath oRootPath;
        ConnectionOptions oCon = new ConnectionOptions();
        ManagementScope oRootMs;
        string Hostname = null;
        private string LoggedOnUserName = null;
        private List<string> _macAddresses = new List<string>();

        public WMIProvider(string hostname)
        {
            oRootPath = new ManagementPath(string.Format("\\\\{0}\\ROOT\\CIMV2", hostname));
            oCCMPath = new ManagementPath(string.Format("\\\\{0}\\ROOT\\CCM", hostname));
            oCon.Impersonation = ImpersonationLevel.Impersonate;
            oCon.EnablePrivileges = true;
            oRootMs = new ManagementScope(oRootPath, oCon);
            this.Hostname = hostname;
            
        }

       
        
        public ComputerStates GetHostStatus()
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

            int timeout = 120;
            try
            {
                PingReply reply = pingSender.Send(this.Hostname, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool HostDNSIsCorrect()
        {
            try
            {
                //Check that the remote host is the current right one!
                ObjectQuery oQuery = new ObjectQuery("SELECT DNSHostName FROM Win32_ComputerSystem");
                ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oRootMs, oQuery);
                ManagementObjectCollection moc = oSearcher.Get();
                if (moc.Count != 0)
                {
                    foreach (ManagementObject mo in moc)
                    {
                        string result = mo["DNSHostName"].ToString();
                        if (result.ToLower() == Hostname.ToLower())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool HostHasUser()
        {
            try
            {
                //Check that the remote host is the current right one!
                ObjectQuery oQuery = new ObjectQuery("SELECT UserName FROM Win32_ComputerSystem");
                ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oRootMs, oQuery);
                ManagementObjectCollection moc = oSearcher.Get();
                if (moc.Count != 0)
                {
                    foreach (ManagementObject mo in moc)
                    {
                        string result = mo["UserName"].ToString();
                        if (result != "")
                        {
                            this.LoggedOnUserName = result;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                this.LoggedOnUserName = null;
                return false;
            }
        }

        private void GetHostMACAddresses()
        {
            try
            {
                //Check that the remote host is the current right one!
                ObjectQuery oQuery = new ObjectQuery("SELECT MACAddress FROM win32_networkadapter WHERE MACAddress != null and NetEnabled = TRUE");
                ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oRootMs, oQuery);
                ManagementObjectCollection moc = oSearcher.Get();
                if (moc.Count != 0)
                {
                    foreach (ManagementObject mo in moc)
                    {
                        this._macAddresses.Add(mo["MACAddress"].ToString());
                    }
                }
            }
            catch
            {
                
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

    }
}

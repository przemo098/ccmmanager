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
using System.Management;

namespace CCMManager.Automation.WMI
{
    public class ComputerSystem
    {
        #region Internal

        WMI.Provider oWMIProvider;
        ManagementObject oWin32_OperatingSystem;
        ManagementObject oWin32_ComputerSystem;
        ManagementObject oWin32_NetworkAdapter;
        ManagementObjectCollection oWin32_SystemEnvironment;
        List<string> oUsersLoggedOn;

        #endregion //Internal
        
        #region Constructor

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="oProvider">A WMI Provider instance.</param>
        public ComputerSystem(WMI.Provider oProvider)
        {
            oWMIProvider = oProvider;
        }

        #endregion //Constructor

        #region Properties

        /// <summary>
        /// Define wether the properties require reloading.
        /// </summary>
        public bool Reload { get; set; }

        /// <summary>
        /// Return the Win32_OperatingSystem ManagementObject.
        /// </summary>
        public ManagementObject Win32_OperatingSystem
        {
            get
            {
                if ((oWin32_OperatingSystem == null) | Reload)
                {
                    WMI.Provider oProvider = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProvider.mScope.Path.NamespacePath = @"Root\CIMV2";
                    oProvider.mScope.Options.EnablePrivileges = true;
                    ManagementObjectCollection moc = oProvider.ExecuteQuery("Select * from Win32_OperatingSystem");
                    foreach (ManagementObject mo in moc)
                    {
                        oWin32_OperatingSystem = mo;
                        Reload = false;
                        return mo;
                    }
                    return null;
                }
                else
                {
                    return oWin32_OperatingSystem;
                }
            }
        }

        /// <summary>
        /// Return the Win32_ComputerSystem ManagementObject.
        /// </summary>
        public ManagementObject Win32_ComputerSystem
        {
            get
            {
                if ((oWin32_ComputerSystem == null) | Reload)
                {
                    WMI.Provider oProvider = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProvider.mScope.Path.NamespacePath = @"ROOT\CIMV2";
                    oProvider.mScope.Options.EnablePrivileges = true;
                    ManagementObjectCollection moc = oProvider.ExecuteQuery("Select * from Win32_ComputerSystem");
                    foreach (ManagementObject mo in moc)
                    {
                        oWin32_ComputerSystem = mo;
                        Reload = false;
                        return mo;
                    }
                    return null;
                }
                else
                {
                    return oWin32_ComputerSystem;
                }
            }
        }

        public ManagementObject Win32_NetworkAdapter
        {
            get
            {
                if ((oWin32_NetworkAdapter == null) | Reload)
                {
                    WMI.Provider oProvider = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProvider.mScope.Path.NamespacePath = @"ROOT\CIMV2";
                    oProvider.mScope.Options.EnablePrivileges = true;
                    ManagementObjectCollection moc = oProvider.ExecuteQuery("Select * from Win32_NetworkAdapter");
                    foreach (ManagementObject mo in moc)
                    {
                        oWin32_NetworkAdapter = mo;
                        Reload = false;
                        return mo;
                    }
                    return null;
                }
                else
                {
                    return oWin32_NetworkAdapter;
                }
            }
        }

        /// <summary>
        /// Return the Win32_Environment ManagementObject.
        /// </summary>
        public ManagementObjectCollection Win32_Environment
        {
            get
            {
                if ((oWin32_SystemEnvironment == null) | Reload)
                {
                    WMI.Provider oProvider = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProvider.mScope.Path.NamespacePath = @"Root\CIMV2";
                    ManagementObjectCollection moc = oProvider.ExecuteQuery("select * from Win32_Environment where systemvariable='True'");
                    return moc;
                }
                else
                {
                    return oWin32_SystemEnvironment;
                }
            }
        }

        /// <summary>
        /// Return the Windows Directory.
        /// </summary>
        public string WindowsDir
        {
            get
            {
                ManagementObject mo = Win32_OperatingSystem;
                return mo.GetPropertyValue("WindowsDirectory").ToString();
            }
        }

        /// <summary>
        /// Return the Last System Boot Date/Time.
        /// </summary>
        public DateTime LastBootTime
        {
            get
            {
                ManagementObject mo = Win32_OperatingSystem;
                return ManagementDateTimeConverter.ToDateTime(mo.GetPropertyValue("LastBootUpTime").ToString());
            }
        }

        /// <summary>
        /// Return the Date of the last OS Installation.
        /// </summary>
        public DateTime OSInstallDate
        {
            get
            {
                ManagementObject mo = Win32_OperatingSystem;
                return ManagementDateTimeConverter.ToDateTime(mo.GetPropertyValue("InstallDate").ToString());
            }
        }

        /// <summary>
        /// Return the System Drive Letter.
        /// </summary>
        public string SystemDriveLetter
        {
            get
            {
                ManagementObject mo = Win32_OperatingSystem;
                return mo.GetPropertyValue("SystemDrive").ToString();
            }
        }

        /// <summary>
        /// Return the OSCaption.
        /// </summary>
        public string OSCaption
        {
            get
            {
                ManagementObject mo = Win32_OperatingSystem;
                return mo.GetPropertyValue("Caption").ToString();
            }
        }

        /// <summary>
        /// Return the currently logged on username.
        /// </summary>
        public string LoggedOnUser
        {
            get
            {
                ManagementObject mo = Win32_ComputerSystem;
                //return Win32_ComputerSystem.GetPropertyValue("UserName").ToString();
                return mo.GetPropertyValue("UserName").ToString();
            }
        }

        /// <summary>
        /// Return a List<string> of Logged on users.
        /// </summary>
        public List<string> LoggedOnUsers
        {
            get
            {
                List<string> lResult = new List<string>();
                if ((oUsersLoggedOn == null) | Reload)
                {
                    WMI.Provider oProvider = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProvider.mScope.Path.NamespacePath = @"ROOT\CIMV2";
                    oProvider.mScope.Options.EnablePrivileges = true;
                    ManagementObjectSearcher searcherLogonSessions = new ManagementObjectSearcher(oProvider.mScope, new ObjectQuery("select __relpath from win32_process where caption = 'explorer.exe'"));
                    foreach (ManagementObject moLogonSession in searcherLogonSessions.Get())
                    {
                        try
                        {
                            ManagementBaseObject oMBO = moLogonSession.InvokeMethod("GetOwner", null, null);
                            lResult.Add(oMBO["Domain"].ToString() + @"\" + oMBO["User"].ToString());
                        }
                        catch
                        {

                        }
                    }
                    return lResult;
                }
                else
                {
                    return oUsersLoggedOn;
                }
            }
        }

        /// <summary>
        /// Return the Manufacturer.
        /// </summary>
        public string Manufacturer
        {
            get
            {
                ManagementObject mo = Win32_ComputerSystem;
                return mo.GetPropertyValue("Manufacturer").ToString();
            }
        }

        /// <summary>
        /// Return the Model.
        /// </summary>
        public string Model
        {
            get
            {
                ManagementObject mo = Win32_ComputerSystem;
                return mo.GetPropertyValue("Model").ToString();
            }
        }

        public List<string> MACAddresses
        {
            get
            {
                ManagementObject mo = Win32_ComputerSystem;
                return new List<string>();
            }
        }

        #endregion //Properties

        #region Public Functions

        /// <summary>
        /// Force a Logoff.
        /// </summary>
        /// <returns>Returns the result code (UInt32)</returns>
        public UInt32 Logoff()
        {
            ManagementObject mo = Win32_OperatingSystem;
            ManagementBaseObject inParams = mo.GetMethodParameters("Win32Shutdown");
            inParams["Flags"] = 4; //Logoff
            ManagementBaseObject outParams = mo.InvokeMethod("Win32Shutdown", inParams, null);
            return UInt32.Parse(outParams.GetPropertyValue("ReturnValue").ToString());
        }

        /// <summary>
        /// Force a restart.
        /// </summary>
        /// <returns>Returns the result code (UInt32)</returns>
        public UInt32 Restart() 
        {
            ManagementObject mo = Win32_OperatingSystem;
            ManagementBaseObject inParams = mo.GetMethodParameters("Win32Shutdown");
            inParams["Flags"] = 6; //forced restart
            ManagementBaseObject outParams = mo.InvokeMethod("Win32Shutdown", inParams, null);
            return UInt32.Parse(outParams.GetPropertyValue("ReturnValue").ToString());
        }

        /// <summary>
        /// Force a PowerOff.
        /// </summary>
        /// <returns>Returns the result code (UInt32)</returns>
        public UInt32 Shutdown() 
        {
            ManagementObject mo = Win32_OperatingSystem;
            ManagementBaseObject inParams = mo.GetMethodParameters("Win32Shutdown");
            inParams["Flags"] = 12; //Forced poweroff
            ManagementBaseObject outParams = mo.InvokeMethod("Win32Shutdown", inParams, null);
            return UInt32.Parse(outParams.GetPropertyValue("ReturnValue").ToString());
        }

        #endregion

    }
}

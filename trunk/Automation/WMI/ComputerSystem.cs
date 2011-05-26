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
        ManagementObjectCollection oWin32_SystemEnvironment;
        List<string> oUsersLoggedOn;

        #endregion //Internal
        
        #region Constructor

        public ComputerSystem(WMI.Provider oProvider)
        {
            oWMIProvider = oProvider;
        }

        #endregion //Constructor

        #region Properties

        public bool Reload { get; set; }

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

        public ManagementObject Win32_ComputerSystem
        {
            get
            {
                if ((oWin32_ComputerSystem == null) | Reload)
                {
                    WMI.Provider oProvider = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProvider.mScope.Path.NamespacePath = @"Root\CIMV2";
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

        public string WindowsDir
        {
            get
            {
                ManagementObject mo = Win32_OperatingSystem;
                return mo.GetPropertyValue("WindowsDirectory").ToString();
            }
        }
        #endregion //Properties

        #region Public Functions

        public UInt32 Logoff()
        {
            ManagementObject mo = Win32_OperatingSystem;
            ManagementBaseObject inParams = mo.GetMethodParameters("Win32Shutdown");
            inParams["Flags"] = 4; //Logoff
            ManagementBaseObject outParams = mo.InvokeMethod("Win32Shutdown", inParams, null);
            return UInt32.Parse(outParams.GetPropertyValue("ReturnValue").ToString());
        }

        public UInt32 Restart() 
        {
            ManagementObject mo = Win32_OperatingSystem;
            ManagementBaseObject inParams = mo.GetMethodParameters("Win32Shutdown");
            inParams["Flags"] = 6; //forced restart
            ManagementBaseObject outParams = mo.InvokeMethod("Win32Shutdown", inParams, null);
            return UInt32.Parse(outParams.GetPropertyValue("ReturnValue").ToString());
        }

        public UInt32 Shutdown() 
        {
            ManagementObject mo = Win32_OperatingSystem;
            ManagementBaseObject inParams = mo.GetMethodParameters("Win32Shutdown");
            inParams["Flags"] = 12; //Forced poweroff
            ManagementBaseObject outParams = mo.InvokeMethod("Win32Shutdown", inParams, null);
            return UInt32.Parse(outParams.GetPropertyValue("ReturnValue").ToString());
        }

        public DateTime LastBootTime
        {
            get
            {
                ManagementObject mo = Win32_OperatingSystem;
                return ManagementDateTimeConverter.ToDateTime(mo.GetPropertyValue("LastBootUpTime").ToString());
            }
        }

        public DateTime OSInstallDate
        {
            get
            {
                ManagementObject mo = Win32_OperatingSystem;
                return ManagementDateTimeConverter.ToDateTime(mo.GetPropertyValue("InstallDate").ToString());
            }
        }

        public string SystemDriveLetter
        {
            get
            {
                ManagementObject mo = Win32_OperatingSystem;
                return mo.GetPropertyValue("SystemDrive").ToString();
            }
        }

        public string OSCaption
        {
            get
            {
                ManagementObject mo = Win32_OperatingSystem;
                return mo.GetPropertyValue("Caption").ToString();
            }
        }

        public string LoggedOnUser
        {
            get
            {
                ManagementObject mo = Win32_ComputerSystem;
                //return Win32_ComputerSystem.GetPropertyValue("UserName").ToString();
                return mo.GetPropertyValue("UserName").ToString();
            }
        }

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


        //Hardware Details...

        public string Manufacturer
        {
            get
            {
                ManagementObject mo = Win32_ComputerSystem;
                return mo.GetPropertyValue("Manufacturer").ToString();
            }
        }

        public string Model
        {
            get
            {
                ManagementObject mo = Win32_ComputerSystem;
                return mo.GetPropertyValue("Model").ToString();
            }
        }

        
        #endregion //Public Functions
    }
}

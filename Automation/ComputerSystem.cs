using System;
using System.Collections.Generic;
using System.Management;

namespace CCMManager.Automation
{
    public class ComputerSystem
    {
        #region Internal

        WMIProvider oWMIProvider;
        ManagementObject oWin32_OperatingSystem;
        ManagementObject oWin32_ComputerSystem;
        ManagementObjectCollection oWin32_SystemEnvironment;
        List<string> oUsersLoggedOn;

        #endregion //Internal
        
        #region Constructor

        public ComputerSystem(WMIProvider oProvider)
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
                    WMIProvider oProvider = new WMIProvider(oWMIProvider.mScope.Clone());
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
                    WMIProvider oProvider = new WMIProvider(oWMIProvider.mScope.Clone());
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
                    WMIProvider oProvider = new WMIProvider(oWMIProvider.mScope.Clone());
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

        #endregion //Public Functions
    }
}

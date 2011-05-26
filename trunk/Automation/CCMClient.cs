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
using System.Management;

namespace CCMManager.Automation
{
    /// <summary>
    /// Represents a single Config Client Manager Instance.
    /// </summary>
    public class CCMClient : IDisposable
    {
        #region Private Fields

        string sSMSSiteCode;
        string sSMSManagementPoint;
        internal string sSMSVersion = "";
        internal string sLocalSMSPath = "";
        string pHostname;
        string pUsername;
        string pPassword;

        SMS.Schedules oSMSSchedules;
        SMS.SoftwareDistribution oSMSSoftwareDistribution;
        SMS.DesiredConfigurationManagement oSMSDCM;
        SMS.Components oSMSComponents;

        WMI.Provider oWMIProvider;
        WMI.ComputerSystem oWMIComputerSystem;
        WMI.Registry oWMIRegistry;
        WMI.Services oWMIServices;
        WMI.FileIO oWMIFileIO;
        WMI.WindowsInstaller oWMIWindowsInstaller;

        ManagementObject oSMS_Client;
        ManagementObject oCCM_Client;
        ManagementObject oCacheConfig;
        ManagementObject mo_SMS_Authority;

        #endregion //Private Fields

        #region Constructors

        public CCMClient(string hostname)
        {
            connect(hostname, null, null);
        }

        public CCMClient(string hostname, string username, string password)
        {
            connect(hostname, username, password);
        }

        public CCMClient(WMI.Provider wmiProvider)
        {
            connect(wmiProvider);
        }

        #endregion //Constructors

        protected void connect(string sHostname)
        {
            connect(sHostname, null, null);
        }

        protected void connect(string sHostname, string username, string password)
        {
            oWMIProvider = new WMI.Provider(@"\\" + sHostname + @"\ROOT\cimv2", username, password);
            connect(oWMIProvider);
            pHostname = sHostname;
            pUsername = username;
            pPassword = password;
        }

        protected void connect(WMI.Provider wmiProv)
        {
            oWMIProvider = wmiProv;
            oSMSSchedules = new SMS.Schedules(oWMIProvider);
            oWMIRegistry = new WMI.Registry(oWMIProvider);
            oWMIServices = new WMI.Services(oWMIProvider);
            oSMSSoftwareDistribution = new SMS.SoftwareDistribution(oWMIProvider);
            oWMIWindowsInstaller = new WMI.WindowsInstaller(oWMIProvider);
            oWMIFileIO = new WMI.FileIO(oWMIProvider);
            oWMIComputerSystem = new WMI.ComputerSystem(oWMIProvider);
            oSMSComponents = new SMS.Components(oWMIProvider);
            oSMSDCM = new SMS.DesiredConfigurationManagement(oWMIProvider);
        }

        #region Public Properties

        public SMS.Schedules Schedules
        { 
            get { return oSMSSchedules; }
        }

        public SMS.SoftwareDistribution SoftwareDistribution 
        { 
            get { return oSMSSoftwareDistribution; } 
        }

        public WMI.WindowsInstaller WindowsInstaller 
        { 
            get { return oWMIWindowsInstaller; } 
        }

        public WMI.FileIO FileIO 
        { 
            get { return oWMIFileIO; } 
        }

        public WMI.ComputerSystem ComputerSystem 
        { 
            get { return oWMIComputerSystem; }
        }

        public SMS.Components Components 
        { 
            get { return oSMSComponents; }
        }

        public SMS.DesiredConfigurationManagement DCM 
        { 
            get { return oSMSDCM; } 
        }

        internal ManagementObject mSMS_Client
        {
            get
            {
                if ((oSMS_Client == null) | Reload)
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                 	Reload = false;
                 	return oProv.GetObject("SMS_Client=@");
                }
                else
                    return oSMS_Client;
            }
            set
            {
                oSMS_Client = value;
            }
        }

        internal ManagementObject mCCM_Client
        {
            get
            {
                if ((oCCM_Client == null) | Reload)
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                    Reload = false;
                    return oProv.GetObject("CCM_Client=@");
                }
                else
                    return oCCM_Client;
            }
            set
            {
                oCCM_Client = value;
            }
        }

        internal ManagementObject mCacheConfig
        {
            get
            {
                if ((oCacheConfig == null) | Reload)
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";
                    Reload = false;
                    return oProv.GetObject("CacheConfig.ConfigKey='Cache'");
                }
                else
                    return oCacheConfig;
            }
            set
            {
                oCacheConfig = value;
            }
        }

        public bool Reload { get;set; }

        public string SiteCode
        {
            get
            {
                if (sSMSSiteCode != null)
                    return sSMSSiteCode;
                else
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM";

                    sSMSSiteCode = oProv.ExecuteMethod("SMS_Client", "GetAssignedSite").GetPropertyValue("sSiteCode").ToString();
                    return sSMSSiteCode;
                }
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                    sSMSSiteCode = null;
                else
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM";

                    ManagementBaseObject inParams = oProv.GetClass("SMS_Client").GetMethodParameters("SetAssignedSite");
                    inParams["sSiteCode"] = value;
                    oProv.ExecuteMethod("SMS_Client", "SetAssignedSite", inParams);
                    //sSiteCode = value;
                    sSMSSiteCode = null; //to clear the cached code...
                }
            }
        }

        public string ManagementPoint
        {
            get
            {
                if (!string.IsNullOrEmpty(sSMSManagementPoint))
                    return sSMSManagementPoint;
                else
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM";

                    mo_SMS_Authority = oProv.GetObject("SMS_Authority.Name='SMS:" + SiteCode + "'");
                    return mo_SMS_Authority.GetPropertyValue("CurrentManagementPoint").ToString();
                }
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                    sSMSManagementPoint = null;
            }
        }

        public string ProxyManagementPoint
        {
            get
            {
                ManagementObjectCollection MPProxies;
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                MPProxies = oProv.ExecuteQuery("SELECT * FROM SMS_MPProxyInformation Where State = 'Active'");
                foreach (ManagementObject MPProxy in MPProxies)
                {
                    return MPProxy.GetPropertyValue("Name").ToString();
                }
                return null;
            }
        }

        //public string InternetMP
        //{
        //    get
        //    {
        //        string sPort = oWMIRegistry.GetString(2147483650, @"SOFTWARE\Microsoft\SMS\Client\Internet Facing", "Internet MP Hostname", "");
        //        if (string.IsNullOrEmpty(sPort))
        //        {
        //            if (this.ComputerSystem.Win32_ComputerSystem["SystemType"].ToString().ToLower().Contains("x64"))
        //            {
        //                sPort = oWMIRegistry.GetString(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\SMS\Client\Internet Facing", "Internet MP Hostname", "");
        //            }
        //        }

        //        return sPort;
        //    }
        //    set
        //    {
        //        if (!this.ComputerSystem.Win32_ComputerSystem["SystemType"].ToString().ToLower().Contains("x64"))
        //        {
        //            oWMIRegistry.SetStringValue(2147483650, @"SOFTWARE\Microsoft\SMS\Client\Internet Facing", "Internet MP Hostname", value);
        //        }
        //        else
        //        {
        //            oWMIRegistry.SetStringValue(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\SMS\Client\Internet Facing", "Internet MP Hostname", value);
        //        }
        //    }
        //}


        #endregion //Public Properties



        #region Inherited Methods

        

        #endregion //Inherited Methods

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

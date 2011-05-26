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

using System.Management;

namespace CCMManager.Automation.SMS
{
    public class Components
    {
        #region Internal Fields

        private WMI.Provider oWMIProvider;
        private ManagementObject oSMS_SoftwareDistributionClientConfig;

        #endregion //Internal Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oProvider"></param>
        public Components(WMI.Provider oProvider)
        {
            oWMIProvider = oProvider;
        }
        
        #endregion //Constructor

        #region Public Methods

        /// <summary>
        /// The cached CCM_SoftwareDistributionClientConfig Class.
        /// </summary>
        /// <returns>root\ccm\policy\machine\requestedconfig:CCM_SoftwareDistributionClientConfig</returns>
        /// <seealso cref="M:smsclictr.automation.SMSComponents.CCM_SoftwareDistributionClientConfig(System.Boolean)"/>
        public ManagementObject CCM_SoftwareDistributionClientConfig()
        {
            if (oSMS_SoftwareDistributionClientConfig == null)
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";
                ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM CCM_SoftwareDistributionClientConfig");
                foreach (ManagementObject MO in MOC)
                {
                    oSMS_SoftwareDistributionClientConfig = MO;
                    return MO;
                }
                return null;
            }
            else
            {
                return oSMS_SoftwareDistributionClientConfig;
            }

        }

        /// <summary>
        /// The CCM_SoftwareDistributionClientConfig Class
        /// </summary>
        /// <param name="refresh">Refresh the cached CCM_SoftwareDistributionClientConfig Object</param>
        /// <returns>root\ccm\policy\machine\requestedconfig:CCM_SoftwareDistributionClientConfig</returns>
        public ManagementObject CCM_SoftwareDistributionClientConfig(bool refresh)
        {
            if (refresh)
            {
                oSMS_SoftwareDistributionClientConfig = null;
            }
            return CCM_SoftwareDistributionClientConfig();
        }

        /// <summary>
        /// local CCM_ComponentClientConfig policy from requested policy
        /// </summary>
        /// <param name="ComponentName"></param>
        /// <returns>ROOT\ccm\Policy\Machine\RequestedConfig:CCM_ComponentClientConfig</returns>
        public ManagementObjectCollection Component_Requested(string ComponentName)
        {
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\ccm\Policy\Machine\RequestedConfig";
            return oProv.ExecuteQuery("SELECT * FROM CCM_ComponentClientConfig WHERE ComponentName = '" + ComponentName + "'");
        }

        /// <summary>
        /// CCM_ComponentClientConfig from actual policy
        /// </summary>
        /// <param name="ComponentName"></param>
        /// <returns>ROOT\ccm\Policy\Machine\ActualConfig:CCM_ComponentClientConfig</returns>
        public ManagementObject Component_Actual(string ComponentName)
        {
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\ccm\Policy\Machine\ActualConfig";
            ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM CCM_ComponentClientConfig WHERE ComponentName = '" + ComponentName + "'");
            foreach (ManagementObject MO in MOC)
            {
                return MO;
            }
            return null;
        }

        #endregion //Public Methods

        #region Public Properties

        /// <summary>
        /// Get the status or disable SoftwareDistribution
        /// </summary>
        public bool DisableSoftwareDistribution
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SoftwareDistributionClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;

            }
            set
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM ccm_SoftwareDistributionClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("ccm_SoftwareDistributionClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsSoftwareDistribution");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("LockSettings", "true");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");

            }
        }

        /// <summary>
        /// Get the status or disable SoftwareMeetering
        /// </summary>
        public bool DisableSoftwareMeetering
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SoftwareMeteringClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_SoftwareMeteringClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_SoftwareMeteringClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsSoftwareMetering");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// Get the status or disable MSISourceUpdate
        /// </summary>
        public bool DisableMSISourceUpdate
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SourceUpdateClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_SourceUpdateClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_SourceUpdateClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsSourceUpdateAgent");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// Get the status or disable Inventory
        /// </summary>
        public bool DisableInventory
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_InventoryClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_InventoryClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_InventoryClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsInventory");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// Get the status or disable remote tools
        /// </summary>
        public bool DisableRemoteTools
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_RemoteToolsConfig.Type=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_RemoteToolsConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_RemoteToolsConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsRemoteTools");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("Type", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// SMSSystemHealthAgent status
        /// </summary>
        public bool DisableHealthAgent
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SystemHealthClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_SystemHealthClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_SystemHealthClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SMSSystemHealthAgent");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// SoftwareUpdatesClientConfig Agent status
        /// </summary>
        public bool DisableSoftwareUpdate
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SoftwareUpdatesClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_SoftwareUpdatesClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_SoftwareUpdatesClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsSoftwareUpdate");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// ConfigurationManagementClientConfig Agent status
        /// </summary>
        public bool DisableConfigurationManagement
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_ConfigurationManagementClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_ConfigurationManagementClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_ConfigurationManagementClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SMSConfigurationManagementAgent");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// ClientAgentConfig Status
        /// </summary>
        public bool ClientAgentConfig
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_ClientAgentConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;

            }
            set
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_ClientAgentConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_ClientAgentConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "ClientAgent");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// Disable OutOfBand
        /// </summary>
        public bool DisableOOB
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_OutOfBandManagementClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;

            }
            set
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_OutOfBandManagementClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_OutOfBandManagementClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsOutOfBandManagement");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// return all instances of ROOT\ccm:CCM_InstalledComponent
        /// </summary>
        public ManagementObjectCollection CCM_InstalledComponent
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\ccm";
                return oProv.ExecuteQuery("SELECT * FROM CCM_InstalledComponent");
            }
        }

        
        

        #endregion //Public Properties
    }
}

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

using System.Collections.Generic;
using System.Management;
using System.Xml;

namespace CCMManager.Automation.SMS
{
    public class DesiredConfigurationManagement
    {
        #region Private Fields

        WMI.Provider oWMIProvider;
        ManagementObjectCollection oDCMBaselines;
        
        #endregion //Private Fields

        #region Constructor

        /// <summary>
        /// DesiredConfigurationManagement Constructor
        /// </summary>
        /// <param name="oProvider">A WMIProvider Instance</param>
        public DesiredConfigurationManagement(WMI.Provider oProvider)
        {
            oWMIProvider = new WMI.Provider(oProvider.mScope.Clone());
        }

        #endregion //Constructor

        #region Public Methods

        /// <summary>
        /// Get DCM Baselines (Class SMS_DesiredConfiguration)
        /// </summary>
        /// <returns>Class SMS_DesiredConfiguration</returns>
        public ManagementObjectCollection CCM_DCMBaselines()
        {
            if (oDCMBaselines == null)
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\dcm";
                ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM SMS_DesiredConfiguration");
                oDCMBaselines = MOC;
                return MOC;
            }
            else
            {
                return oDCMBaselines;
            }

        }

        /// <summary>
        /// Get DCM Baselines (Class SMS_DesiredConfiguration)
        /// </summary>
        /// <param name="refresh">true=reload;false=get cached objects</param>
        /// <returns>Class SMS_DesiredConfiguration</returns>
        public ManagementObjectCollection CCM_DCMBaselines(bool refresh)
        {
            if (refresh)
            {
                oDCMBaselines = null;
            }
            return CCM_DCMBaselines();
        }

        /// <summary>
        /// List of Config Items (Class ConfigItem)
        /// </summary>
        /// <param name="DCMBaseline">Instance of SMS_DesiredConfiguration ManagementObject</param>
        /// <returns>Confg Items</returns>
        public List<ConfigItem> ConfigItems(ManagementObject DCMBaseline)
        {
            List<ConfigItem> oResult = new List<ConfigItem>();
            try
            {
                XmlDocument xDoc = new XmlDocument();
                DCMBaseline.Get();
                xDoc.LoadXml(DCMBaseline.Properties["ComplianceDetails"].Value.ToString());
                XmlNodeList xNodes = xDoc.SelectNodes(@"//DiscoveryReport/BaselineCIComplianceState/PartsCompliance/PartCIComplianceState");
                foreach (XmlNode xNode in xNodes)
                {
                    ConfigItem oItem = new ConfigItem();
                    oItem.LogicalName = xNode.Attributes["LogicalName"].Value.ToString();
                    oItem.Applicable = bool.Parse(xNode.Attributes["Applicable"].Value.ToString());
                    oItem.Compliant = bool.Parse(xNode.Attributes["Compliant"].Value.ToString());
                    oItem.Detected = bool.Parse(xNode.Attributes["Detected"].Value.ToString());
                    oItem.Type = xNode.Attributes["Type"].Value.ToString();
                    oItem.Version = xNode.Attributes["Version"].Value.ToString();

                    oItem.CIName = xNode.SelectSingleNode("./CIProperties/LocalizableText[@PropertyName='CIName']").InnerText;
                    oItem.CIDescription = xNode.SelectSingleNode("./CIProperties/LocalizableText[@PropertyName='CIDescription']").InnerText;

                    if (xNode.SelectSingleNode("./ConstraintViolations[@Count > 0]") != null)
                    {
                        oItem.ConstraintViolation = xNode.SelectSingleNode("./ConstraintViolations/ConstraintViolation").Attributes["Severity"].Value.ToString();
                    }
                    else
                    {
                        oItem.ConstraintViolation = "";
                    }
                    oResult.Add(oItem);
                }
            }
            catch { }
            return oResult;
        }

        #endregion //Public Methods

        /// <summary>
        /// ConfigItem Object
        /// </summary>
        public class ConfigItem
        {
            #region Public Properties

            /// <summary>
            /// Logical Name
            /// </summary>
            public string LogicalName { get; set; }

            /// <summary>
            /// Display Name
            /// </summary>
            public string CIName { get; set; }

            /// <summary>
            /// Description
            /// </summary>
            public string CIDescription { get; set; }

            /// <summary>
            /// CI Version
            /// </summary>
            public string Version { get; set; }

            /// <summary>
            /// CI Type
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// Compliance state
            /// </summary>
            public bool Compliant { get; set; }

            /// <summary>
            /// Detection state
            /// </summary>
            public bool Detected { get; set; }

            /// <summary>
            /// Applicable state
            /// </summary>
            public bool Applicable { get; set; }

            /// <summary>
            /// Violation status
            /// </summary>
            public string ConstraintViolation { get; set; }

            #endregion //Public Properties
        }

        
    }
}

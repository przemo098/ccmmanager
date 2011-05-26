﻿//CCMManager
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Xml;

namespace CCMManager.Automation.SMS
{
    public class SoftwareDistribution
    {
        #region Internal Fields

        private WMI.Provider oWMIProvider;
        private List<ManagementObject> ladvertisements = new List<ManagementObject>();
        
        #endregion //Internal Fields

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="oProvider">WMI.Provider Instance</param>
        public SoftwareDistribution(WMI.Provider oProvider)
        {
            oWMIProvider = oProvider;
        }
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Get a ManagementObjectCollection of all Machine SoftwareDistribution Policies (ActualConfig)
        /// </summary>
        /// <remarks>root\ccm\Policy\Machine\ActualConfig:SELECT * FROM CCM_SoftwareDistribution
        /// </remarks>
        public ManagementObjectCollection Advertisements
        {
            get
            {
                EnumerationOptions oOptions = new EnumerationOptions();
                //oOptions.ReturnImmediately = true;
                //doOptions.Rewindable = true;
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\Machine\ActualConfig";
                return oProv.ExecuteQuery("SELECT * FROM CCM_SoftwareDistribution", oOptions);
            }
        }

        /// <summary>
        /// List of Advertisments (cached)
        /// </summary>
        public List<ManagementObject> lAdvertisements
        {
            get
            {
                return ladvertisements;
            }
            set
            {
                ladvertisements = lAdvertisements;
            }
        }

        /// <summary>
        /// Get a ManagementObjectCollection of all Machine SoftwareDistribution Policies running when a user is logged on (RequestedConfig)
        /// </summary>
        /// <remarks>root\ccm\Policy\Machine\RequestedConfig:SELECT * FROM CCM_SoftwareDistribution WHERE PRG_PRF_UserLogonRequirement = 'UserLoggedOn' and PRG_HistoryLocation = 'User'
        /// </remarks>
        public ManagementObjectCollection Advertisements_UserLogon
        {
            get
            {
                EnumerationOptions oOptions = new EnumerationOptions();
                oOptions.ReturnImmediately = true;
                oOptions.Rewindable = true;
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\Machine\RequestedConfig";
                return oProv.ExecuteQuery("SELECT * FROM CCM_SoftwareDistribution WHERE PRG_PRF_UserLogonRequirement = 'UserLoggedOn' and PRG_HistoryLocation = 'User'", oOptions);
            }
        }

        /// <summary>
        /// Get all running executions
        /// </summary>
        /// <returns>ManagementObjectCollection from the WMI Class root\ccm\SoftMgmtAgent:CCM_ExecutionRequest</returns>
        public ManagementObjectCollection RunningExecutions
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";
                EnumerationOptions oEnumOpt = new EnumerationOptions();
                oEnumOpt.ReturnImmediately = true;
                oEnumOpt.Rewindable = false;
                if (isSCCMClient)
                {
                    return oProv.ExecuteQuery("SELECT * FROM CCM_ExecutionRequestEx", oEnumOpt);
                }
                else
                {
                    return oProv.ExecuteQuery("SELECT * FROM CCM_ExecutionRequest", oEnumOpt);
                }

            }
        }

        /// <summary>
        /// Get all running executions with status "ReportStatusAtReboot"
        /// </summary>
        /// <returns>ManagementObjectCollection from the WMI Class root\ccm\SoftMgmtAgent:CCM_ExecutionRequest where State = 'ReportStatusAtReboot'</returns>
        public ManagementObjectCollection ReportStatusAtReboot
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";
                EnumerationOptions oEnumOpt = new EnumerationOptions();
                oEnumOpt.ReturnImmediately = true;
                oEnumOpt.Rewindable = false;
                return oProv.ExecuteQuery("SELECT * FROM CCM_ExecutionRequest where State = 'ReportStatusAtReboot'", oEnumOpt);
            }
        }

        /// <summary>
        /// Get all running downloads
        /// </summary>
        /// <returns>ManagementObjectCollection from the WMI Class root\ccm\SoftMgmtAgent:CCM_ExecutionRequest</returns>
        public ManagementObjectCollection RunningDownloads
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";

                EnumerationOptions oEnumOpt = new EnumerationOptions();
                oEnumOpt.ReturnImmediately = true;
                oEnumOpt.Rewindable = false;
                if (isSCCMClient)
                {
                    return oProv.ExecuteQuery("SELECT * FROM DownloadInfoEx2", oEnumOpt);
                }
                else
                {
                    return oProv.ExecuteQuery("SELECT * FROM DownloadInfo", oEnumOpt);
                }
            }
        }

        /// <summary>
        /// Get the ExecutionRequest with the state "Running".
        /// If there is no Running Excution the function returns a NULL value
        /// </summary>
        /// <remarks>
        /// Returns a ManagementObject from the WMI Class root\ccm\SoftMgmtAgent:CCM_ExecutionRequest
        /// <code>
        /// [key] string ProgramID
        /// [key] string TargetUser
        /// [key] string AdvertID
        /// [key] string ContentID
        /// string ContentVersion
        /// string State
        /// boolean DependeePolicyExists
        /// string RunningState
        /// string ContentRequestGuid
        /// datetime ReceivedTime
        /// datetime NextRetryTime
        /// uint32 RetryCount
        /// uint32 ContentAccessRetryCount
        /// uint32 RetryInterval
        /// uint32 ProgramExitCode
        /// boolean RunOnCompletion
        /// boolean RunInQuietMode
        /// boolean IsAdminContext
        /// boolean DownloadStartedNotified
        /// boolean ProgramReboot
        /// boolean MIFChecking
        /// string MIFFileName
        /// string MIFPackageName
        /// string MIFPackageVersion
        /// string MIFPackagePublisher
        /// string ExecutionContextTempPath
        /// string OptionalAdvertisements[]
        /// boolean DependencyCheckEvaluated
        /// boolean IgnoreRunRerunFlags
        /// uint32 ProcessID
        /// uint32 ProcessCreationTimeLow
        /// uint32 ProcessCreationTimeHigh
        /// string LastStatusMessage
        /// </code>
        /// </remarks>
        public ManagementObject RunningAdv
        {
            get
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";

                EnumerationOptions oEnumOpt = new EnumerationOptions();
                oEnumOpt.ReturnImmediately = true;
                oEnumOpt.Rewindable = false;
                if (isSCCMClient)
                {
                    ManagementObjectCollection MOC = oProv.ExecuteQuery(@"SELECT * FROM CCM_ExecutionRequestEx WHERE State='Running'", oEnumOpt);
                    foreach (ManagementObject MO in MOC)
                    {
                        return MO;
                    }
                }
                else
                {
                    ManagementObjectCollection MOC = oProv.ExecuteQuery(@"SELECT * FROM CCM_ExecutionRequest WHERE State='Running'", oEnumOpt);
                    foreach (ManagementObject MO in MOC)
                    {
                        return MO;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Check if an Advertisement is running (boolean)
        /// </summary>
        public Boolean AdvIsRunning
        {
            get
            {
                if (RunningAdv != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Check if Client has an SCCM Agent installed
        /// </summary>
        public Boolean isSCCMClient
        {
            get
            {
                string sVersion;
                try
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                    sVersion = oProv.GetObject("SMS_Client=@").GetPropertyValue("ClientVersion").ToString();
                    if (sVersion.StartsWith("4."))
                        return true;
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
        }


        #endregion //Public Properties

        #region Internal Methods

        /// <summary>
        /// Import SCCM Policy from XML Node
        /// </summary>
        /// <param name="xClass"></param>
        /// <param name="sNamespacePath"></param>
        /// <param name="bPersistent"></param>
        /// <returns></returns>
        internal ManagementObject iImportSCCMPolicy(XmlNode xClass, string sNamespacePath, bool bPersistent)
        {
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = sNamespacePath;
            try
            {
                ManagementClass MC = oProv.GetClass(xClass.Attributes["class"].Value);
                ManagementObject MO = MC.CreateInstance();

                foreach (XmlNode xProp in xClass.ChildNodes)
                {
                    try
                    {
                        if (MO.Properties[xProp.Attributes["name"].Value].IsArray)
                        {

                            String[] aText = new String[xProp.ChildNodes.Count];
                            int i = 0;
                            foreach (XmlNode xNode in xProp.ChildNodes)
                            {
                                aText[i] = xNode.InnerText;
                                i++;
                            }
                            MO.SetPropertyValue(xProp.Attributes["name"].Value, aText);
                        }
                        else
                        {
                            string sValue = xProp.InnerText.Replace("\r\n\t", "");
                            sValue = sValue.Replace("\t\t", "").Trim();
                            MO.SetPropertyValue(xProp.Attributes["name"].Value, sValue);
                        }
                    }
                    catch { }
                }

                if (string.Compare(MO.SystemProperties["__CLASS"].Value.ToString(), "CCM_SoftwareDistribution", true) == 0)
                {
                    if (bPersistent)
                        MO.SetPropertyValue("PolicySource", "LOCAL");
                    MO.SetPropertyValue("ADV_MandatoryAssignments", true);
                }

                //MO.Put();
                return MO;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Merges the XML data into local policy
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="bPersistent"></param>
        /// <returns></returns>
        internal ManagementObject oImpSCCMPol(XmlDocument xDoc, bool bPersistent)
        {
            string sPolicyID = xDoc.DocumentElement.Attributes["PolicyID"].Value;

            ManagementObject MO = new ManagementObject();
            ManagementObject MORet = new ManagementObject();
            XmlNodeList xActions = xDoc.GetElementsByTagName("PolicyAction");
            foreach (XmlNode xNode in xActions)
            {
                if (string.Compare(xNode.Attributes["PolicyActionType"].Value, "WMI-XML", true) == 0)
                {
                    string sRuleId = xNode.ParentNode.Attributes["PolicyRuleID"].Value;
                    XmlNodeList xClasses = xNode.ChildNodes;

                    foreach (XmlNode xClass in xClasses)
                    {
                        /*ManagementObject MOPol = ImportSCCMPolicy(xClass, @"root\ccm\Policy", false);
                        MOPol.SetPropertyValue("PolicyID", sPolicyID);
                        MOPol.SetPropertyValue("PolicyRuleID", sRuleId);
                        MOPol.Put(); */

                        //Load Policy in RequestedConfig
                        ManagementObject MOReq = iImportSCCMPolicy(xClass, @"root\ccm\Policy\Machine\RequestedConfig", bPersistent);
                        MOReq.SetPropertyValue("PolicyID", sPolicyID);
                        MOReq.SetPropertyValue("PolicyRuleID", sRuleId);
                        MOReq.Put();

                        //Load Policy in ActualConfig
                        MO = iImportSCCMPolicy(xClass, @"root\ccm\Policy\Machine\ActualConfig", false);
                        MO.Put();

                        if (string.Compare(MO.SystemProperties["__Class"].Value.ToString(), "CCM_SoftwareDistribution", true) == 0)
                        {
                            MO.Get();
                            MORet = MO;
                        }
                    }
                }
            }
            return MORet;
        }

        #endregion //Internal Methods

        #region Public Methods

        /// <summary>
        /// Get all Advertisements from a User
        /// </summary>
        /// <param name="SID">Security Identifier</param>
        /// <returns></returns>
        public ManagementObjectCollection GetAdvertisements(string SID)
        {
            EnumerationOptions oOptions = new EnumerationOptions();
            //oOptions.ReturnImmediately = true;
            //oOptions.Rewindable = true;
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = string.Format(@"root\ccm\Policy\{0}\ActualConfig", SID);
            ManagementObjectCollection oResult = oProv.ExecuteQuery("SELECT * FROM CCM_SoftwareDistribution", oOptions);

            //Cache Adv...
            foreach (ManagementObject MO in oResult)
            {
                if (!lAdvertisements.Contains(MO))
                {
                    ladvertisements.Add(MO);
                }
            }

            return oResult;

        }

        /// <summary>
        /// Remove local persistent SoftwareDistribution policies
        /// </summary>
        /// <remarks>Wait or enforce a PolicyEvaluation cycle to enforce the process</remarks>
        public void CleanupPersistentPolicies()
        {
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\CCM\Policy\Machine\RequestedConfig";
            oProv.DeleteQueryResults("SELECT * FROM CCM_SoftwareDistribution WHERE PolicySource ='LOCAL'");
            oProv.DeleteQueryResults("SELECT * FROM CCM_Scheduler_ScheduledMessage WHERE PolicySource ='LOCAL'");
        }

        /// <summary>
        /// Get a local Advertisement policy as ManagementObject
        /// </summary>
        /// <param name="ADV_AdvertisementId">Advertisement ID</param>
        /// <param name="PKG_PackageId">Package ID</param>
        /// <param name="PRG_ProgramId">Program ID (Program Name)</param>
        /// <returns>A ManagementObject with the local Advertisement Settings (root\ccm\Policy\Machine\ActualConfig:CCM_SoftwareDistribution)</returns>
        /// <remarks>
        /// <para>Policy Properties:</para>
        /// </remarks>
        public ManagementObject GetAdvert(string ADV_AdvertisementId, string PKG_PackageId, string PRG_ProgramId)
        {
            return GetAdvert(ADV_AdvertisementId, PKG_PackageId, PRG_ProgramId, "Machine");
        }

        /// <summary>
        /// Import and assign a local Software Distribution Policy
        /// </summary>
        /// <param name="PolicyPath">Path to the XML File or URL on the SMS ManagementPoint</param>
        /// <remarks>Policy will automatically be removed with the next policy evalutaion cycle</remarks>
        /// <returns>CCM_SoftwareDistribution ManagementObject</returns>
        public ManagementObject ImportLocalPolicy(string PolicyPath)
        {
            return ImportLocalPolicy(PolicyPath, false);
        }

        /// <summary>
        /// Import a local Software Distribution Policy
        /// </summary>
        /// <param name="PolicyPath">Path to the XML File or URL on the SMS ManagementPoint</param>
        /// <param name="bPersistent">Create a persistant policy</param>
        /// <remarks>Persistant policies will not be removed automatically. Only a full/hard policy reset will remove these policies</remarks>
        /// <returns>CCM_SoftwareDistribution ManagementObject</returns>
        public ManagementObject ImportLocalPolicy(string PolicyPath, bool bPersistent)
        {
            string PolicyID = "";
            string PolicyVersion = "";
            ManagementObject SWDist = new ManagementObject();
            try
            {

                XmlTextReader reader = new XmlTextReader(PolicyPath);
                reader.WhitespaceHandling = WhitespaceHandling.None;

                #region Extract MOF Data from XML
                while (!reader.EOF)
                {
                    reader.Read();
                    if ((reader.Name == "Policy") & (reader.NodeType == XmlNodeType.Element))
                    {
                        PolicyID = reader.GetAttribute("PolicyID");
                        PolicyVersion = reader.GetAttribute("PolicyVersion");
                    }
                    if (reader.Name == "PolicyAction" & (reader.GetAttribute("PolicyActionType") == "WMI-MOF"))
                    {
                        reader.Read();
                        if ((reader.NodeType == XmlNodeType.CDATA) & (reader.HasValue))
                        {

                            using (StreamWriter sw = new StreamWriter(Environment.ExpandEnvironmentVariables(@"%TEMP%\") + PolicyID + ".mof"))
                            {
                                sw.WriteLine(reader.Value);
                            }
                        }
                    }
                }
                reader.Close();
                #endregion

                #region Compile MOF to WMI
                //Compile the extracted MOF File to the local WMI Namespace root\ccm\policy ...
                Process mofcomp = new Process();
                mofcomp.StartInfo.FileName = "mofcomp";
                mofcomp.StartInfo.Arguments = @"-N:root\ccm\policy " + Environment.ExpandEnvironmentVariables(@"%TEMP%\") + PolicyID + ".mof";
                mofcomp.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                mofcomp.Start();
                mofcomp.WaitForExit();
                if (!mofcomp.HasExited)
                {
                    mofcomp.Kill();
                }
                mofcomp.Dispose();
                #endregion

                #region Create remote policy

                //Copy the compiled local Policies...
                ManagementScope LocalScope = new ManagementScope(@"root\ccm\policy");
                LocalScope.Connect();

                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy";
                SelectQuery Query = new SelectQuery("SELECT * FROM CCM_Scheduler_ScheduledMessage WHERE PolicyID='" + PolicyID + "'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(LocalScope, Query);
                ManagementObjectCollection SchedColl = searcher.Get();
                foreach (ManagementObject MO in SchedColl)
                {
                    WMI.Provider.ManagementObjectCopy(MO, oProv.mScope, new ManagementPath(@"\\" + oProv.mScope.Path.Server + @"\root\ccm\policy:CCM_Scheduler_ScheduledMessage"));
                }

                Query = new SelectQuery("SELECT * FROM CCM_SoftwareDistribution WHERE PolicyID='" + PolicyID + "'");
                searcher = new ManagementObjectSearcher(LocalScope, Query);
                ManagementObjectCollection SWDistColl = searcher.Get();
                foreach (ManagementObject MO in SWDistColl)
                {
                    SWDist = MO;
                    if (bPersistent) MO.SetPropertyValue("PolicySource", "LOCAL");
                    MO.SetPropertyValue("ADV_MandatoryAssignments", "True");
                    WMI.Provider.ManagementObjectCopy(MO, oProv.mScope, new ManagementPath(@"\\" + oProv.mScope.Path.Server + @"\root\ccm\policy:CCM_SoftwareDistribution"));
                }

                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\actualconfig";
                foreach (ManagementObject MO in SchedColl)
                {

                    //MO.SetPropertyValue("PolicySource", "LOCAL"); 
                    WMI.Provider.ManagementObjectCopy(MO, oProv.mScope, new ManagementPath(@"\\" + oProv.mScope.Path.Server + @"\root\ccm\policy\machine\actualconfig:CCM_Scheduler_ScheduledMessage"));
                }

                foreach (ManagementObject MO in SWDistColl)
                {
                    //MO.SetPropertyValue("PolicySource", "LOCAL");
                    WMI.Provider.ManagementObjectCopy(MO, oProv.mScope, new ManagementPath(@"\\" + oProv.mScope.Path.Server + @"\root\ccm\policy\machine\actualconfig:CCM_SoftwareDistribution"));
                }
                if (bPersistent)
                {
                    oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\RequestedConfig";
                    foreach (ManagementObject MO in SchedColl)
                    {
                        MO.SetPropertyValue("PolicySource", "LOCAL");
                        WMI.Provider.ManagementObjectCopy(MO, oProv.mScope, new ManagementPath(@"\\" + oProv.mScope.Path.Server + @"\root\ccm\policy\machine\RequestedConfig:CCM_Scheduler_ScheduledMessage"));
                    }

                    foreach (ManagementObject MO in SWDistColl)
                    {
                        MO.SetPropertyValue("PolicySource", "LOCAL");
                        MO.SetPropertyValue("ADV_MandatoryAssignments", "True");
                        WMI.Provider.ManagementObjectCopy(MO, oProv.mScope, new ManagementPath(@"\\" + oProv.mScope.Path.Server + @"\root\ccm\policy\machine\RequestedConfig:CCM_SoftwareDistribution"));
                    }
                }


                #endregion

                #region Delete local Policy
                foreach (ManagementObject MO in SWDistColl)
                {
                    MO.Delete();
                }
                foreach (ManagementObject MO in SchedColl)
                {
                    MO.Delete();
                }

                #endregion

                #region Delete remote Policy
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy";
                oProv.DeleteQueryResults("SELECT * FROM CCM_Scheduler_ScheduledMessage WHERE PolicyID='" + PolicyID + "'");
                oProv.DeleteQueryResults("SELECT * FROM CCM_SoftwareDistribution WHERE PolicyID='" + PolicyID + "'");
                #endregion

                #region delete local .MOF File
                System.IO.File.Delete(Environment.ExpandEnvironmentVariables(@"%TEMP%\") + PolicyID + ".mof");
                #endregion
            }
            catch
            {
                throw;
            }
            return SWDist;
        }

        /// <summary>
        /// Get a local Advertisement policy as ManagementObject
        /// </summary>
        /// <param name="ADV_AdvertisementId">Advertisement ID</param>
        /// <param name="PKG_PackageId">Package ID</param>
        /// <param name="PRG_ProgramId">Program ID (Program Name)</param>
        /// <returns>A ManagementObject with the local Advertisement Settings (root\ccm\Policy\Machine\ActualConfig:CCM_SoftwareDistribution)</returns>
        /// <remarks>
        /// <para>Policy Properties:</para>
        /// <code>
        /// ADV_ActiveTime
        /// ADV_ActiveTimeIsGMT
        /// ADV_ADF_Published
        /// ADV_AdvertisementID
        /// ADV_FirstRunBehavior
        /// ADV_MandatoryAssignments
        /// ADV_RCF_InstallFromLocalDPOptions
        /// ADV_RCF_InstallFromRemoteDPOptions
        /// ADV_RCF_PostponeToAC
        /// ADV_RepeatRunBehavior
        /// PKG_ContentSize
        /// PKG_Language
        /// PKG_Manufacturer
        /// PKG_MIFChecking
        /// PKG_MifFileName
        /// PKG_MIFName
        /// PKG_MIFPublisher
        /// PKG_MIFVersion
        /// PKG_Name
        /// PKG_PackageID
        /// PKG_PSF_ContainsSourceFiles
        /// PKG_SourceHash
        /// PKG_SourceVersion
        /// PKG_version
        /// PRG_CommandLine
        /// PRG_Comment
        /// PRG_DependentPolicy
        /// PRG_ForceDependencyRun
        /// PRG_HistoryLocation
        /// PRG_MaxDuration
        /// PRG_PRF_AfterRunning
        /// PRG_PRF_Disabled
        /// PRG_PRF_InstallsApplication
        /// PRG_PRF_MappedDriveRequired
        /// PRG_PRF_PersistMappedDrive
        /// PRG_PRF_RunWithAdminRights
        /// PRG_PRF_ShowWindow
        /// PRG_PRF_UserInputRequired
        /// PRG_PRF_UserLogonRequirement
        /// PRG_ProgramID
        /// PRG_ProgramName
        /// PRG_Requirements
        /// PRG_ReturnCodesSource
        /// PRG_WorkingDirectory
        /// </code>
        /// </remarks>
        public ManagementObject GetAdvert(string ADV_AdvertisementId, string PKG_PackageId, string PRG_ProgramId, string SID)
        {
            try
            {
                ManagementObject MO = ladvertisements.Find(p => (p.Properties["ADV_AdvertisementID"].Value.ToString() == ADV_AdvertisementId) & (p.Properties["PKG_PackageID"].Value.ToString() == PKG_PackageId) & (p.Properties["PRG_ProgramID"].Value.ToString() == PRG_ProgramId));
                if (MO != null)
                {
                    return MO;
                }

                SID = SID.Replace("-", "_");

                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\" + SID + @"\ActualConfig";
                MO = oProv.GetObject("CCM_SoftwareDistribution.ADV_AdvertisementID='" + ADV_AdvertisementId + "',PKG_PackageID='" + PKG_PackageId + "',PRG_ProgramID='" + PRG_ProgramId + "'");
                ladvertisements.Add(MO);
                return MO;

            }
            catch
            {
                throw new System.Exception("Unable to get advertisement");
            }
        }

        /// <summary>
        /// ReRun a failed Advertisement
        /// </summary>
        /// <param name="ADV_AdvertisementID">Advertisement ID</param>
        /// <param name="PKG_PackageID">Package ID</param>
        /// <param name="PRG_ProgramID">Program ID (Program Name)</param>
        /// <remarks>Rerun an Advertisement only if Adv. is not installed or was previously failed</remarks>
        public void RerunAdvIfMissing(string ADV_AdvertisementID, string PKG_PackageID, string PRG_ProgramID)
        {
            ArrayList ScheduleIDs = new ArrayList();
            ScheduleIDs.AddRange(GetSchedulIDs(ADV_AdvertisementID, PKG_PackageID, PRG_ProgramID));
            SMS.Schedules SMSSched = new SMS.Schedules(oWMIProvider);
            foreach (string sID in ScheduleIDs)
            {
                try
                {
                    SMSSched.TriggerScheduleID(sID);
                }
                catch { }
            }

        }

        /// <summary>
        /// Get all ScheduleID's of a local Advertisement
        /// </summary>
        /// <param name="ADV_AdvertisementID">Advertisment ID</param>
        /// <param name="PKG_PackageID">Package ID</param>
        /// <param name="PRG_ProgramID">Program ID (Program Name)</param>
        /// <returns>String Array with ScheduleID's for the defined Advertisement (ScheduleID Example:"LAB20001-LAB00003-D34DE188"</returns>
        /// <remarks>Only scheduled Advertisements (DateTime/As soon as possible/..) do have a ScheduleID</remarks>
        public string[] GetSchedulIDs(string ADV_AdvertisementID, string PKG_PackageID, string PRG_ProgramID)
        {
            return GetSchedulIDs(ADV_AdvertisementID, PKG_PackageID, PRG_ProgramID, "Machine");
        }

        /// <summary>
        /// Import a local Software Distribution Policy for SCCM Clients
        /// </summary>
        /// <param name="PolicyBody">Byte[] Policy Body</param>
        /// <param name="bPersistent">Create a persistant policy</param>
        /// <remarks>Persistant policies will not be removed automatically. Only a full/hard policy reset will remove these policies</remarks>
        /// <returns>CCM_SoftwareDistribution ManagementObject</returns>
        public ManagementObject ImportSCCMPolicy(byte[] PolicyBody, bool bPersistent)
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                System.IO.Stream str = new System.IO.MemoryStream(PolicyBody);
                xDoc.Load(str);

                return oImpSCCMPol(xDoc, bPersistent);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Import a local Software Distribution Policy for SCCM Clients
        /// </summary>
        /// <param name="sPolicyPath">Path to the XML File or URL on the SMS ManagementPoint</param>
        /// <param name="bPersistent">Create a persistant policy</param>
        /// <remarks>Persistant policies will not be removed automatically. Only a full/hard policy reset will remove these policies</remarks>
        /// <returns>CCM_SoftwareDistribution ManagementObject</returns>
        public ManagementObject ImportSCCMPolicy(string sPolicyPath, bool bPersistent)
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(sPolicyPath);
                return oImpSCCMPol(xDoc, bPersistent);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Get all ScheduleID's of a local Advertisement
        /// </summary>
        /// <param name="ADV_AdvertisementID">Advertisment ID</param>
        /// <param name="PKG_PackageID">Package ID</param>
        /// <param name="PRG_ProgramID">Program ID (Program Name)</param>
        /// <param name="UserSID">User Secirity Identifier (SID) or NULL for Machine</param>
        /// <returns></returns>
        public string[] GetSchedulIDs(string ADV_AdvertisementID, string PKG_PackageID, string PRG_ProgramID, string UserSID)
        {
            List<string> SchedID = new List<string>();
            //Chekc if User or Machine is requested
            if (string.IsNullOrEmpty(UserSID))
                UserSID = "Machine";
            else
                UserSID = UserSID.Replace('-', '_');

            try
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = string.Format(@"root\ccm\Policy\{0}\ActualConfig", UserSID);
                ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM CCM_Scheduler_ScheduledMessage WHERE ScheduledMessageID like '" + ADV_AdvertisementID + "-" + PKG_PackageID + "%'");
                foreach (ManagementObject MO in MOC)
                {
                    try
                    {
                        SchedID.Add(MO.Properties["ScheduledMessageID"].Value.ToString());

                        MO.Properties["Triggers"].Value = new string[] { "OneShot;MaxRandomDelay=0" };
                        MO.Put();
                    }
                    catch { }
                }

                //W2k Support removed ...

                return SchedID.ToArray();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// ReRun a scheduled Advertisement
        /// </summary>
        /// <param name="ADV_AdvertisementID">Advertisement ID</param>
        /// <param name="PKG_PackageID">Package ID</param>
        /// <param name="PRG_ProgramID">Program ID (Program Name)</param>
        /// <returns>The restarted ScheduleID or NULL if the Advertisement does not have a valid ScheduleID</returns>
        /// <remarks>Only scheduled Advertisements (DateTime/As soon as possible/..) which are assigned to a Client can be restarted</remarks>
        public string RerunAdv(string ADV_AdvertisementID, string PKG_PackageID, string PRG_ProgramID)
        {
            return RerunAdv(ADV_AdvertisementID, PKG_PackageID, PRG_ProgramID, "");
        }

        /// <summary>
        /// ReRun a scheduled Advertisement
        /// </summary>
        /// <param name="ADV_AdvertisementID">Advertisement ID</param>
        /// <param name="PKG_PackageID">Package ID</param>
        /// <param name="PRG_ProgramID">Program ID (Program Name)</param>
        /// <param name="UserSID">User Security Identifier (SID) or null for Machine</param>
        /// <returns>The restarted ScheduleID or NULL if the Advertisement does not have a valid ScheduleID</returns>
        /// <remarks>Only scheduled Advertisements (DateTime/As soon as possible/..) which are assigned to a Client can be restarted</remarks>
        public string RerunAdv(string ADV_AdvertisementID, string PKG_PackageID, string PRG_ProgramID, string UserSID)
        {
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());

            //Chekc if User or Machine is requested
            if (string.IsNullOrEmpty(UserSID))
                UserSID = "Machine";
            else
                UserSID = UserSID.Replace('-', '_');

            oProv.mScope.Path.NamespacePath = string.Format(@"root\ccm\Policy\{0}\ActualConfig", UserSID);
            ManagementObject oAdv = oProv.GetObject(@"CCM_SoftwareDistribution.ADV_AdvertisementID='" + ADV_AdvertisementID + "',PKG_PackageID='" + PKG_PackageID + "',PRG_ProgramID='" + PRG_ProgramID + "'");

            oAdv.SetPropertyValue("ADV_MandatoryAssignments", "True");
            oAdv.SetPropertyValue("ADV_RepeatRunBehavior", "RerunAlways");
            oAdv.Put();

            ArrayList ScheduleIDs = new ArrayList();
            ScheduleIDs.AddRange(GetSchedulIDs(ADV_AdvertisementID, PKG_PackageID, PRG_ProgramID, UserSID));
            SMS.Schedules SMSSched = new SMS.Schedules(oProv);
            foreach (string sID in ScheduleIDs)
            {
                try
                {
                    SMSSched.TriggerScheduleID(sID, false);
                    return sID;
                }
                catch
                {
                }
            }
            return null;

        }

        /// <summary>
        /// Get a all Advertisements from a specifed PackageID and ProgramName
        /// </summary>
        /// <param name="PKG_PackageId"></param>
        /// <param name="PRG_ProgramId"></param>
        /// <returns></returns>
        public ManagementObjectCollection GetAdverts(string PKG_PackageId, string PRG_ProgramId)
        {
            try
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\Machine\ActualConfig";
                return oProv.ExecuteQuery("SELECT * FROM CCM_SoftwareDistribution WHERE PKG_PackageID = '" + PKG_PackageId + "' and PRG_ProgramID = '" + PRG_ProgramId + "'");
            }
            catch
            {
                throw new System.Exception("Unable to get advertisements");
            }
        }

        /// <summary>
        /// Get a all Advertisements (cached) from a specifed PackageID and ProgramName
        /// </summary>
        /// <param name="PKG_PackageId"></param>
        /// <param name="PRG_ProgramId"></param>
        /// <returns></returns>
        public List<ManagementObject> lGetAdverts(string PKG_PackageId, string PRG_ProgramId)
        {
            try
            {
                List<ManagementObject> oResult = new List<ManagementObject>();

                //Check if ADV is Cached...
                oResult = ladvertisements.FindAll(p => (p.Properties["PKG_PackageID"].Value.ToString() == PKG_PackageId) & (p.Properties["PRG_ProgramID"].Value.ToString() == PRG_ProgramId));
                if (oResult.Count > 0)
                {
                    return oResult;
                }
                else
                {

                    //Adv is not cached..
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\Machine\ActualConfig";
                    ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM CCM_SoftwareDistribution WHERE PKG_PackageID = '" + PKG_PackageId + "' and PRG_ProgramID = '" + PRG_ProgramId + "'");
                    foreach (ManagementObject MOSW in MOC)
                    {
                        ladvertisements.Add(MOSW);
                        oResult.Add(MOSW);
                    }
                }

                return oResult;
            }
            catch
            {
                throw new System.Exception("Unable to get advertisements");
            }
        }
        
        #endregion //Public Methods

    }
}

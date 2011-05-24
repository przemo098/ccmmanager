using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace CCMManager.Automation
{
    public class SMSSchedules
    {
        #region Internal

        WMIProvider oWMIPrivider;
        ManagementClass mcSMSClass;
        ManagementBaseObject inParams;
        string sDCMScanSchedlueId;

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.  Initiates the WMIProvider object.
        /// </summary>
        /// <param name="oProvider">A WMIProvider object.</param>
        public SMSSchedules(WMIProvider oProvider)
        {
            oWMIPrivider = new WMIProvider(oProvider.mScope.Clone());
        }

        #endregion //Constructors
        
        #region Public Functions

        public void RunHardwareInventory()
        {
            RunHardwareInventory(false);
        }

        public void RunHardwareInventory(bool doFullInventory)
        {
            if (doFullInventory)
            {
                this.DeleteInventoryHistory("{00000000-0000-0000-0000-000000000001}");
            }
            TriggerScheduleID("{00000000-0000-0000-0000-000000000001}", true);
        }

        public void RunSoftwareInventory()
        {
            RunSoftwareInventory(false);
        }

        public void RunSoftwareInventory(bool doFullInventory)
        {
            if (doFullInventory)
            {
                DeleteInventoryHistory("{00000000-0000-0000-0000-000000000002}");
            }
            TriggerScheduleID("{00000000-0000-0000-0000-000000000002}", true);
        }

        public void RunDataDiscovery()
        {
            TriggerScheduleID("{00000000-0000-0000-0000-000000000003}", true);
        }

        public void RunSoftwareInventoryFileCollection()
        {
            TriggerScheduleID("{00000000-0000-0000-0000-000000000010}");
        }

        public void RunMachinePolicyRetrievalEval()
        {
            TriggerScheduleID("{00000000-0000-0000-0000-000000000021}", true);
        }

        public void RunMachinePolicyEval()
        {
            TriggerScheduleID("{00000000-0000-0000-0000-000000000022}", true);
        }

        public void RunSoftwareMeteringReport()
        {
            TriggerScheduleID("{00000000-0000-0000-0000-000000000031}");
        }

        public void RunMSISourceUpdate()
        {
            TriggerScheduleID("{00000000-0000-0000-0000-000000000032}");
        }

        public void RunPolicyCleanup()
        {
            TriggerScheduleID("{00000000-0000-0000-0000-000000000040}");
        }

        public void RunAssignmentValidation()
        {
            TriggerScheduleID("{00000000-0000-0000-0000-000000000042}");
        }

        public void RunSUSEvalCycle()
        {
            TriggerScheduleID("{00000000-0000-0000-0000-000000000108}");
        }

        public void SendUnsentMessages()
        {
            TriggerScheduleID("{00000000-0000-0000-0000-000000000111}");
        }

        public void CleanMessageCache()
        {
            TriggerScheduleID("{00000000-0000-0000-0000-000000000112}");
        }

        public void RunDCMScan()
        {
            if (string.IsNullOrEmpty(sDCMScanSchedlueId))
            {
                ManagementScope mScope = this.oWMIPrivider.mScope.Clone();
                mScope.Path.NamespacePath = @"ROOT\CCM\policy\machine\actualconfig";
                ManagementObjectCollection moc = new ManagementObjectSearcher(mScope, new ObjectQuery("SELECT * FROM CCM_Scheduler_ScheduledMessage WHERE TargetEndpoint = 'direct:DCMAgent'")).Get();
                foreach (ManagementObject mo in moc)
                {
                    try
                    {
                        string id = mo["ScheduledMessageID"].ToString();
                        if (!string.IsNullOrEmpty(id))
                        {
                            sDCMScanSchedlueId = id;
                            if (string.IsNullOrEmpty(sDCMScanSchedlueId))
                            {
                                //Error
                            }
                            else
                            {
                                TriggerScheduleID(sDCMScanSchedlueId);
                            }
                        }
                    }
                    catch
                    { }
                }
            }
        }

        public void TriggerScheduleID(string scheduleId)
        {
            TriggerScheduleID(scheduleId, false);
        }

        public void TriggerScheduleID(string scheduleId, bool updateHistory)
        {
            WMIProvider oProvider = new WMIProvider(oWMIPrivider.mScope.Clone());
            oProvider.mScope.Path.NamespacePath = @"ROOT\CCM";
            mcSMSClass = oProvider.GetClass("SMS_Client");
            inParams = mcSMSClass.GetMethodParameters("TriggerSchedule");
            try
            {
                inParams["sScheduleID"] = scheduleId;
                oProvider.ExecuteMethod("SMS_Client", "TriggerSchedule", inParams);
            }
            catch
            { }

            if (updateHistory)
            {
                oProvider.mScope.Path.NamespacePath = @"ROOT\CCM\Scheduler";
                ManagementObject mo = null;
                try
                {
                    mo = oProvider.GetObject("CCM_Scheduler_History.ScheduleID='"+scheduleId+"',UserSID='Machine'");
                }
                catch
                {
                }
                if (mo == null)
                {
                    mo = oProvider.GetClass("CCM_Scheduler_History").CreateInstance();
                    mo.SetPropertyValue("ScheduleID", scheduleId);
                    mo.SetPropertyValue("UserSID", "Machine");
                    mo.SetPropertyValue("FirstEvalTime", System.Management.ManagementDateTimeConverter.ToDmtfDateTime(DateTime.Now.ToUniversalTime()));
                }
                mo.SetPropertyValue("LastTriggerTime", System.Management.ManagementDateTimeConverter.ToDmtfDateTime(DateTime.Now.ToUniversalTime()));
                mo.Put();
            }
        }

        #endregion //Public Functions

        #region Private Functions

        private void DeleteInventoryHistory(string scheduleId)
        {
            try
            {

            }
            catch
            {

            }
        }

        #endregion //Private Functions

        #region Public Properties

        public DateTime LastHardwareInventory
        {
            get
            {
                WMIProvider oProvider = new WMIProvider(oWMIPrivider.mScope.Clone());
                oProvider.mScope.Path.NamespacePath = @"ROOT\CCM\Scheduler";
                ManagementObject mo = oProvider.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000001}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(mo.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        public DateTime LastSoftwareInventory
        {
            get
            {
                WMIProvider oProvider = new WMIProvider(oWMIPrivider.mScope.Clone());
                oProvider.mScope.Path.NamespacePath = @"ROOT\CCM\Scheduler";
                ManagementObject mo = oProvider.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000002}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(mo.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        public DateTime LastDataDiscoveryCycle
        {
            get
            {
                WMIProvider oProvider = new WMIProvider(oWMIPrivider.mScope.Clone());
                oProvider.mScope.Path.NamespacePath = @"ROOT\CCM\Scheduler";
                ManagementObject mo = oProvider.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000003}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(mo.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        public DateTime LastPolicyDownloadEval
        {
            get
            {
                WMIProvider oProvider = new WMIProvider(oWMIPrivider.mScope.Clone());
                oProvider.mScope.Path.NamespacePath = @"ROOT\CCM\Scheduler";
                ManagementObject mo = oProvider.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000021}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(mo.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        public DateTime LastPolicyEvaluation
        {
            get
            {
                WMIProvider oProvider = new WMIProvider(oWMIPrivider.mScope.Clone());
                oProvider.mScope.Path.NamespacePath = @"ROOT\CCM\Scheduler";
                ManagementObject mo = oProvider.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000022}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(mo.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        public DateTime LastSUSUpdateAssignmentEval
        {
            get
            {
                WMIProvider oProvider = new WMIProvider(oWMIPrivider.mScope.Clone());
                oProvider.mScope.Path.NamespacePath = @"ROOT\CCM\Scheduler";
                ManagementObject mo = oProvider.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000108}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(mo.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        //public DateTime LastSoftwareInstall
        //{
        //    get
        //    {
                //Reg
        //    }
        //}


        #endregion //Public Properties

    }
}

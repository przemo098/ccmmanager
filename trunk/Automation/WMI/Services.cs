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
using System.Collections;
using System.Management;
using System.Threading;

namespace CCMManager.Automation.WMI
{
    public class Services : IDisposable
    {
        #region Internal
        
        WMI.Provider oWMIProvider;
        ManualResetEvent _mre = new ManualResetEvent(false);

        #endregion //Internal

        #region Constructors

        public Services(WMI.Provider oProv)
        {
            oWMIProvider = oProv;
        }

        #endregion //Constructors

        #region Public Methods

        public ArrayList StopService(string ServiceName)
        {
            try
            {
                ManagementObjectCollection Dependencies;
                ManagementObject Service;
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\cimv2";
                oProv.mScope.Options.EnablePrivileges = true;
                Service = oProv.GetObject("Win32_Service.Name='" + ServiceName + "'");
                Dependencies = oProv.ExecuteQuery("Associators of {Win32_Service.Name='" + ServiceName + "'} Where AssocClass=Win32_DependentService Role=Antecedent");

                ArrayList Result = new ArrayList();
                foreach (ManagementObject MO in Dependencies)
                {
                    if (MO.GetPropertyValue("State").ToString().ToLower() == "running")
                    {
                        Result.AddRange(StopService(MO.GetPropertyValue("Name").ToString()));
                    }
                }
                Result.Add(ServiceName);
                bStopService(Service);
                return Result;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public int StartService(String ServiceName)
        {
            ManagementObject Service;
            try
            {
                ManagementEventWatcher watcher = new ManagementEventWatcher();
                lock (oWMIProvider)
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\cimv2";
                    oProv.mScope.Options.EnablePrivileges = true;
                    Service = oProv.GetObject("Win32_Service.Name='" + ServiceName + "'");

                    WqlEventQuery query = new WqlEventQuery("__InstanceModificationEvent");
                    query.Condition = "TargetInstance ISA 'Win32_Service' AND TargetInstance.Name='" + ServiceName + "' AND TargetInstance.State='Running'";
                    query.WithinInterval = new TimeSpan(0, 0, 2);
                    watcher = new ManagementEventWatcher(oProv.mScope, query);
                    watcher.EventArrived += new EventArrivedEventHandler(ServiceEventArrivedHandler);
                }
                //watcher.Options.Timeout = new TimeSpan(0, 0, 15);
                watcher.Start();

                Object result = Service.InvokeMethod("StartService", null);
                int iResult = int.Parse(result.ToString());
                if (iResult == 0)
                {
                    _mre.WaitOne(new TimeSpan(0, 0, 30), true);
                }
                watcher.Stop();
                watcher.Dispose();

                Service.Get();


                if (Service["State"].ToString() == "Running")
                {
                    return iResult;
                }
                else
                {
                    return iResult;
                }
            }
            catch (Exception ex)
            {
                //0x80080005 wmi stopped
                throw (ex);
            }
        }

        public bool StartService(ArrayList ServiceNames)
        {
            Boolean Result = true;
            foreach (string ServiceName in ServiceNames)
            {
                if (StartService(ServiceName) == 0)
                {
                    Result = false;
                }
            }
            return Result;

        }

        public ManagementObject GetService(String ServiceName)
        {
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";

            return oProv.GetObject("Win32_Service.Name='" + ServiceName + "'");
        }

        public int SetServiceStartMode(string ServiceName, string StartMode)
        {
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            ManagementObject Service = oProv.GetObject("Win32_Service.Name='" + ServiceName + "'");
            ManagementBaseObject inParams = Service.GetMethodParameters("ChangeStartMode");
            inParams["StartMode"] = StartMode;
            ManagementBaseObject Result = Service.InvokeMethod("ChangeStartMode", inParams, null);
            return int.Parse(Result.GetPropertyValue("ReturnValue").ToString());
        }

        public int SetServiceStartMode(ManagementObject Service, string StartMode)
        {
            ManagementBaseObject inParams = Service.GetMethodParameters("ChangeStartMode");
            inParams["StartMode"] = StartMode;
            ManagementBaseObject Result = Service.InvokeMethod("ChangeStartMode", inParams, null);
            return int.Parse(Result.GetPropertyValue("ReturnValue").ToString());
        }

        public bool RestartService(String ServiceName)
        {
            return StartService(StopService(ServiceName));
        }

        public int KillProcess(string ProcessName)
        {
            ManagementObjectCollection MOC;
            int Res = 0;
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\CIMV2";
            oProv.mScope.Options.EnablePrivileges = true;
            MOC = oProv.ExecuteQuery("SELECT * FROM Win32_Process WHERE Name='" + ProcessName + "'");

            foreach (ManagementObject MO in MOC)
            {
                ManagementBaseObject inParams = MO.GetMethodParameters("Terminate");
                Res = int.Parse((MO.InvokeMethod("Terminate", inParams, null)).GetPropertyValue("ReturnValue").ToString());
            }
            return Res;

        }

        public int KillProcess(int ProcessID)
        {
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            oProv.mScope.Options.EnablePrivileges = true;
            ManagementObject MO = oProv.GetObject("Win32_Process.Handle='" + ProcessID.ToString() + "'");
            ManagementBaseObject inParams = MO.GetMethodParameters("Terminate");
            ManagementBaseObject Res = MO.InvokeMethod("Terminate", inParams, null);
            return int.Parse(Res.GetPropertyValue("ReturnValue").ToString());
        }

        public int StartProcess(string CommandLine, string CurrentDirectory, ManagementBaseObject ProcessStartupInformation)
        {

            if (CurrentDirectory == "")
                CurrentDirectory = null;

            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            ManagementClass MC = oProv.GetClass("Win32_Process");
            ManagementBaseObject inParams = MC.GetMethodParameters("Create");
            inParams["CommandLine"] = CommandLine;
            inParams["CurrentDirectory"] = CurrentDirectory;
            inParams["ProcessStartupInformation"] = ProcessStartupInformation;

            ManagementBaseObject Result = MC.InvokeMethod("Create", inParams, null);

            switch (int.Parse(Result.GetPropertyValue("ReturnValue").ToString()))
            {
                case 0: return int.Parse(Result.GetPropertyValue("ProcessID").ToString());
                case 2: throw new System.Security.SecurityException("Access denied");
                case 3: throw new System.Security.SecurityException("Insufficient privilege");
                case 9: throw new Exception("Path not found: " + CommandLine);
                case 21: throw new Exception("Invalid parameter");
                default: throw new Exception("Unknown failure");
            }
        }

        //public bool AdjustDebugPrivileges()
        //{
        //    TokPriv1Luid tp;
        //    bool Result;
        //    IntPtr hproc = GetCurrentProcess();
        //    IntPtr htok = IntPtr.Zero;
        //    Result = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
        //    tp.Count = 1;
        //    tp.Luid = 0;
        //    tp.Attr = SE_PRIVILEGE_ENABLED;
        //    Result = LookupPrivilegeValue(null, SE_DEBUG_NAME, ref tp.Luid);
        //    Result = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
        //    return Result;
        //}
        
        #endregion //Public Methods

        #region Private Methods

        private void ServiceEventArrivedHandler(object sender, EventArrivedEventArgs args)
        {
            try
            {
                ManagementBaseObject mbo = (ManagementBaseObject)args.NewEvent["TargetInstance"];
            }
            catch
            {

            }
            finally
            {
                _mre.Set();
            }
        }

        private bool bStopService(ManagementObject Service)
        {
            try
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\CIMV2";

                WqlEventQuery query = new WqlEventQuery("__InstanceModificationEvent");
                query.Condition = "TargetInstance ISA 'Win32_Service' AND TargetInstance.Name='" + Service.GetPropertyValue("Name").ToString() + "' AND TargetInstance.State='Stopped'";
                query.WithinInterval = new TimeSpan(0, 0, 2);
                ManagementEventWatcher watcher = new ManagementEventWatcher(oProv.mScope, query);
                watcher.EventArrived += new EventArrivedEventHandler(ServiceEventArrivedHandler);
                //watcher.Options.Timeout = new TimeSpan(0, 0, 15);
                watcher.Start();
                Object result = Service.InvokeMethod("StopService", null);
                if ((UInt32)result == 0)
                {
                    _mre.WaitOne(new TimeSpan(0, 0, 60), true);
                }
                watcher.Stop();
                watcher.Dispose();
                Service.Get();

                if (Service["State"].ToString() == "Stopped")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        #endregion //Private Methods

        #region Inherited Methods

        void IDisposable.Dispose()
        {
            
        }

        #endregion //Inherited MEthods.
    }
}

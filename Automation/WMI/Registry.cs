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
using System.Collections.Generic;
using System.Management;

namespace CCMManager.Automation.WMI
{
    public class Registry
    {
        #region Internal
        
        WMI.Provider oWMIProvider;

        #endregion //Internal

        #region Constructors

        public Registry(WMI.Provider oProv)
        {
            oWMIProvider = oProv;
        }

        #endregion //Constructors

        #region Public Functions

        public string GetDWord(UInt32 hDefKey, string sSubKeyName, string sValueName)
        {
            return GetDWord(hDefKey, sSubKeyName, sValueName, "");
        }

        public string GetDWord(UInt32 hDefKey, string sSubKeyName, string sValueName, string DefaultValue)
        {
            {
                String result = "";
                try
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\default";

                    ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("GetDWORDValue");
                    inParams["hDefKey"] = hDefKey;
                    inParams["sSubKeyName"] = sSubKeyName;
                    inParams["sValueName"] = sValueName;
                    ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "GetDWORDValue", inParams);

                    if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                    {
                        if (outParams.GetPropertyValue("uValue") != null)
                        {
                            result = outParams.GetPropertyValue("uValue").ToString();
                        }
                    }
                    return result;
                }
                catch
                {
                    return DefaultValue;
                }
            }
        }

        public string GetString(UInt32 hDefKey, string sSubKeyName, string sValueName)
        {
            return GetString(hDefKey, sSubKeyName, sValueName, "");
        }

        public string GetString(UInt32 hDefKey, string sSubKeyName, string sValueName, string DefaultValue)
        {
            {
                String result = "";
                try
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\default";
                    ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("GetStringValue");
                    inParams["hDefKey"] = hDefKey;
                    inParams["sSubKeyName"] = sSubKeyName;
                    inParams["sValueName"] = sValueName;
                    ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "GetStringValue", inParams);

                    if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                    {
                        if (outParams.GetPropertyValue("sValue") != null)
                        {
                            result = outParams.GetPropertyValue("sValue").ToString();
                        }
                    }
                    return result;
                }
                catch
                {
                    return DefaultValue;
                }
            }
        }

        public string[] GetMultiString(UInt32 hDefKey, string sSubKeyName, string sValueName, string DefaultValue)
        {
            {
                try
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\default";
                    ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("GetMultiStringValue");
                    inParams["hDefKey"] = hDefKey;
                    inParams["sSubKeyName"] = sSubKeyName;
                    inParams["sValueName"] = sValueName;
                    ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "GetMultiStringValue", inParams);

                    if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                    {
                        if (outParams.GetPropertyValue("sValue") != null)
                        {
                            return outParams.GetPropertyValue("sValue") as string[];
                        }
                    }
                    return new string[] { DefaultValue };
                }
                catch
                {
                    return new string[] { DefaultValue };
                }
            }
        }

        public void SetDWord(UInt32 hDefKey, string sSubKeyName, string sValueName, string sValue)
        {
            SetDWord(hDefKey, sSubKeyName, sValueName, System.Convert.ToUInt32(sValue));
        }

        public void SetDWord(UInt32 hDefKey, string sSubKeyName, string sValueName, UInt32 uValue)
        {
            try
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("SetDWORDValue");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                inParams["sValueName"] = sValueName;
                inParams["uValue"] = uValue;
                ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "SetDWORDValue", inParams);
            }
            catch
            {
                throw;
            }
        }

        public void DeleteKey(UInt32 hDefKey, String sSubKeyName)
        {
            try
            {
                //Delete all subkeys
                ArrayList Subkeys = RegKeys(hDefKey, sSubKeyName);
                foreach (string skey in Subkeys)
                {
                    DeleteKey(hDefKey, sSubKeyName + @"\" + skey);
                }

                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("DeleteKey");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                oProv.ExecuteMethod("StdRegProv", "DeleteKey", inParams);
            }
            catch
            {
                throw;
            }
        }

        public ArrayList RegKeys(UInt32 hDefKey, string sSubKeyName)
        {
            try
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("EnumKey");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "EnumKey", inParams);
                ArrayList result = new ArrayList();
                if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                {
                    if (outParams.GetPropertyValue("sNames") != null)
                    {
                        result.AddRange(outParams.GetPropertyValue("sNames") as String[]);
                    }
                }
                return result;
            }
            catch
            {
                throw;
            }
        }

        public ArrayList RegValues(UInt32 hDefKey, string sSubKeyName)
        {
            try
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("EnumValues");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "EnumValues", inParams);
                ArrayList result = new ArrayList();
                if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                {
                    if (outParams.GetPropertyValue("sNames") != null)
                    {
                        result.AddRange(outParams.GetPropertyValue("sNames") as String[]);
                    }
                }
                return result;
            }
            catch
            {
                throw;
            }
        }

        public List<string> RegValuesList(UInt32 hDefKey, string sSubKeyName)
        {
            try
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("EnumValues");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "EnumValues", inParams);
                List<string> result = new List<string>();
                if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                {
                    if (outParams.GetPropertyValue("sNames") != null)
                    {
                        result.AddRange(outParams.GetPropertyValue("sNames") as String[]);
                    }
                }
                return result;
            }
            catch
            {
                throw;
            }
        }

        public String GetExStringValue(UInt32 hDefKey, String sSubKeyName, String sValueName)
        {
            return GetExStringValue(hDefKey, sSubKeyName, sValueName, "");
        }

        public String GetExStringValue(UInt32 hDefKey, String sSubKeyName, String sValueName, String DefaultValue)
        {
            try
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("GetExpandedStringValue");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                inParams["sValueName"] = sValueName;
                ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "GetExpandedStringValue", inParams);
                String result = "";
                if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                {
                    if (outParams.GetPropertyValue("sValue") != null)
                    {
                        result = outParams.GetPropertyValue("sValue").ToString();
                    }
                }
                return result;
            }
            catch
            {
                return DefaultValue;
            }
        }

        public void SetExStringValue(UInt32 hDefKey, String sSubKeyName, String sValueName, String sValue)
        {
            try
            {
                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("SetExpandedStringValue");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                inParams["sValueName"] = sValueName;
                inParams["sValue"] = sValue;
                oProv.ExecuteMethod("StdRegProv", "SetExpandedStringValue", inParams);
            }
            catch
            {
                throw;
            }
        }

        public void SetStringValue(UInt32 hDefKey, String sSubKeyName, String sValueName, String sValue)
        {
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\default";
            ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("SetStringValue");
            inParams["hDefKey"] = hDefKey;
            inParams["sSubKeyName"] = sSubKeyName;
            inParams["sValueName"] = sValueName;
            inParams["sValue"] = sValue;
            oProv.ExecuteMethod("StdRegProv", "SetStringValue", inParams);
        }

        #endregion //Public Functions
    }
}

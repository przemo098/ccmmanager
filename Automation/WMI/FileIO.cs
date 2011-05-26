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

namespace CCMManager.Automation.WMI
{
    public class FileIO
    {
        #region Internal Fields

        private WMI.Provider oWMIProvider;

        #endregion //Internal Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oProvider"></param>
        public FileIO(WMI.Provider oProvider)
        {
            oWMIProvider = oProvider;
        }

        #endregion //Constructor

        #region Public Methods

        /// <summary>
        /// Delete a single File
        /// </summary>
        /// <param name="FilePath"></param>
        public void DeleteFile(string FilePath)
        {
            WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            ManagementObject MO = oProv.GetObject("CIM_DataFile.Name='" + FilePath + "'");
            MO.InvokeMethod("Delete", null);
        }

        /// <summary>
        /// Get an ArrayList of all Subfolders
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public ArrayList SubFolders(string Path)
        {
            lock (oWMIProvider)
            {
                ArrayList result = new ArrayList();
                try
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\cimv2";
                    ManagementObjectCollection MOC = oProv.ExecuteQuery(@"Associators of {Win32_Directory.Name='" + Path + @"'} where AssocClass=Win32_Subdirectory ResultRole=PartComponent");

                    foreach (ManagementObject MO in MOC)
                    {
                        try
                        {
                            result.Add(MO.GetPropertyValue("Name").ToString().ToLower());
                        }
                        catch { }
                    }
                    return result;
                }
                catch { }

                return result;

            }

        }

        /// <summary>
        /// Delete a Folder with all Subfolders and Files
        /// </summary>
        /// <param name="Path"></param>
        public void DeleteFolder(string Path)
        {
            if (!string.IsNullOrEmpty(Path))
            {
                try
                {
                    ManagementObjectCollection MOC;

                    //Delete all Subfolders
                    foreach (string sSub in SubFolders(Path))
                    {
                        DeleteFolder(sSub);
                    }
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\cimv2";
                    string FolderPath = Path.Replace(@"\", @"\\");
                    //ManagementObjectCollection MOC = oWMIProvider.ExecuteQuery("SELECT * FROM Win32_Directory WHERE Drive like '" + (FolderPath.Split('\\'))[0] + "' and Path like '" + (CachePath.Split(':'))[1] + @"\\' and FileType = 'File Folder'");
                    MOC = oProv.ExecuteQuery("SELECT * FROM Win32_Directory WHERE Name = '" + FolderPath + "'");

                    //Delete the root Folder
                    foreach (ManagementObject MO in MOC)
                    {
                        try
                        {
                            ManagementBaseObject inParams = MO.GetMethodParameters("DeleteEx");
                            ManagementBaseObject result = MO.InvokeMethod("DeleteEx", inParams, null);
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Check if Directory Exists
        /// </summary>
        /// <param name="Path"></param>
        /// <returns>True = Directory exists</returns>
        public Boolean DirExist(string Path)
        {
            if (!string.IsNullOrEmpty(Path))
            {
                try
                {
                    WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                    string FolderPath = Path.Replace(@"\", @"\\");
                    ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM Win32_Directory WHERE Name = '" + FolderPath + "'");
                    foreach (ManagementObject MO in MOC)
                    {
                        return true;
                    }
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// Delete multiple Files
        /// </summary>
        /// <param name="Drive">Disk Drive like 'c:'</param>
        /// <param name="Path">Path like '\\windows\\'</param>
        /// <param name="Filename">Filename like 'kb%'</param>
        /// <param name="Extension">Extension like 'log'</param>
        public void DeleteFiles(string Drive, string Path, string Filename, string Extension)
        {
            try
            {
                ManagementObjectCollection MOC;

                if (!Path.EndsWith(@"\"))
                    Path = Path + @"\";

                WMI.Provider oProv = new WMI.Provider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\cimv2";
                string FolderPath = Path.Replace(@"\", @"\\");
                //ManagementObjectCollection MOC = oWMIProvider.ExecuteQuery("SELECT * FROM Win32_Directory WHERE Drive like '" + (FolderPath.Split('\\'))[0] + "' and Path like '" + (CachePath.Split(':'))[1] + @"\\' and FileType = 'File Folder'");
                MOC = oProv.ExecuteQuery(string.Format("SELECT * FROM CIM_DataFile WHERE Drive = '{0}' and Path = '{1}' and Filename like '{2}' and Extension like '{3}'", new object[] { Drive, FolderPath, Filename, Extension }));

                //Delete the root Folder
                foreach (ManagementObject MO in MOC)
                {
                    try
                    {
                        ManagementBaseObject inParams = MO.GetMethodParameters("Delete");
                        ManagementBaseObject result = MO.InvokeMethod("Delete", inParams, null);
                    }
                    catch { }
                }
            }
            catch { }
        }
        
        #endregion //Public Methods
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace CCMManager.Automation
{
    public class WMIProvider
    {
        #region Internal

        /// <summary>
        /// Holds reference to the current ManagementScope
        /// </summary>
        private ManagementScope _mScope;
        public ManagementScope mScope
        {
            get { return _mScope; }
            set 
            { 
                _mScope = value; 
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Connect to a given namespace.
        /// </summary>
        /// <param name="sNamespace">Namespace to be connected to.</param>
        public WMIProvider(string sNamespace)
        {
            Connect(sNamespace);
        }

        /// <summary>
        /// Connect to a given ManagementScope.
        /// </summary>
        /// <param name="mScope">ManagementScope object to be connected to.</param>
        public WMIProvider(ManagementScope mScope)
        {
            mScope = mScope.Clone();
        }

        /// <summary>
        /// Connect to a given namespace with specified credentials.
        /// </summary>
        /// <param name="sNamespace">Namespace to connect to.</param>
        /// <param name="sUser">Username to be used.</param>
        /// <param name="sPassword">Password for the usernaem.</param>
        public WMIProvider(string sNamespace, string sUser, string sPassword)
        {
            Connect(sNamespace, sUser, sPassword);
        }

        #endregion //Constructors

        #region Public Methods

        #region Connect

        /// <summary>
        /// Connect to a given Namespace.
        /// </summary>
        /// <param name="sNamespace">Namespace to connect to.</param>
        public void Connect(string sNamespace)
        {
            mScope = new ManagementScope(sNamespace);
            mScope.Connect();
        }

        /// <summary>
        /// Connect to a given Namespace with specified credentials.
        /// </summary>
        /// <param name="sNameSpace">Namespace to connect to.</param>
        /// <param name="sUser">Username to connect with.</param>
        /// <param name="sPassword">Password for the specified user.</param>
        public void Connect(string sNameSpace, string sUser, string sPassword)
        {
            if (!string.IsNullOrEmpty(sNameSpace))
            {
                ConnectionOptions conOptions = new ConnectionOptions();
                if (sNameSpace.ToUpper().StartsWith("\\\\" + System.Environment.MachineName.ToUpper() + "\\") == false)
                {
                    conOptions.Username = sUser;
                    conOptions.Password = sPassword;
                }
                mScope = new ManagementScope(sNameSpace, conOptions);
                mScope.Connect();
            }

        }

        /// <summary>
        /// Connect to a managementscope.
        /// </summary>
        public void Connect()
        {
            mScope.Connect();
        }

        #endregion //Connect

        #region GetObject

        /// <summary>
        /// Get a ManagementObject with specified path and ObjectGetOptions.
        /// </summary>
        /// <param name="path">The path to return the object from.</param>
        /// <param name="oGetOptions">The options to use.</param>
        /// <returns>Returns a ManagementObject.</returns>
        public ManagementObject GetObject(string path, ObjectGetOptions oGetOptions)
        {
            ManagementObject result = new ManagementObject(mScope, new ManagementPath(path), oGetOptions);
            result.Get();
            return result;
        }

        /// <summary>
        /// Get a ManagementObject with specified path
        /// </summary>
        /// <param name="path">The path to return the object from.</param>
        /// <returns>Returns a ManagemenObject</returns>
        public ManagementObject GetObject(string path)
        {
            return GetObject(path, new ObjectGetOptions());
        }

        #endregion //GetObject

        #region GetClass

        /// <summary>
        /// Gets a ManagementClass given the class name and ObjectGetOPtions.
        /// </summary>
        /// <param name="sClassName">The classname to return.</param>
        /// <param name="oGetOptions">The objectgetoptions to use.</param>
        /// <returns>Returns a ManagementClass</returns>
        public ManagementClass GetClass(string sClassName, ObjectGetOptions oGetOptions)
        {
            ManagementClass result = new ManagementClass(mScope, new ManagementPath(sClassName), oGetOptions);
            return result;
        }

        /// <summary>
        /// Get a Class given its name.
        /// </summary>
        /// <param name="sClassName">The classname to return.</param>
        /// <returns>Returns a ManagementClass.</returns>
        public ManagementClass GetClass(string sClassName)
        {
            return GetClass(sClassName, new ObjectGetOptions());
        }

        #endregion //GetClass

        #region ExecuteQuery

        /// <summary>
        /// Execute a given Query
        /// </summary>
        /// <param name="sQuery">The Query to be executed.</param>
        /// <returns>Returns a ManagementObjectCollection.</returns>
        public ManagementObjectCollection ExecuteQuery(string sQuery)
        {
            ManagementObjectSearcher oSrch = new ManagementObjectSearcher(mScope, new ObjectQuery(sQuery));
            return oSrch.Get();
        }

        /// <summary>
        /// Execute a given Query with the provided EnumerationOptions.
        /// </summary>
        /// <param name="sQuery">The Query to be executed.</param>
        /// <param name="oEnumOpts">The EnumerationOptions to be used.</param>
        /// <returns>Returns a ManagementObjectCollection.</returns>
        public ManagementObjectCollection ExecuteQuery(string sQuery, EnumerationOptions oEnumOpts)
        {
            ManagementObjectSearcher oSrch = new ManagementObjectSearcher(mScope, new ObjectQuery(sQuery), oEnumOpts);
            return oSrch.Get();
        }

        #endregion //ExecuteQuery

        #region ExecuteMethod

        /// <summary>
        /// Executes a Method with a given path, methodname and inParams.
        /// </summary>
        /// <param name="path">The Path to be used.</param>
        /// <param name="method">The method name to be executed.</param>
        /// <param name="inParams">the InParams for the specified method.</param>
        /// <returns>Returns a ManagementBaseObject</returns>
        public ManagementBaseObject ExecuteMethod(string path, string method, ManagementBaseObject inParams)
        {
            ManagementClass oClass = GetClass(path);
            oClass.Get();
            return oClass.InvokeMethod(method, inParams, new InvokeMethodOptions());
        }

        /// <summary>
        /// Executes a Method with a given path and methodname.
        /// </summary>
        /// <param name="path">The Path to be used.</param>
        /// <param name="method">The method name to be executed.</param>
        /// <returns>Returns a ManagementBaseObject</returns>
        public ManagementBaseObject ExecuteMethod(string path, string method)
        {
            ManagementClass oClass = GetClass(path);
            oClass.Get();
            ManagementBaseObject inParams = oClass.GetMethodParameters(method);
            return oClass.InvokeMethod(method, inParams, new InvokeMethodOptions());
        }

        /// <summary>
        /// Executes a method with a given Class and method name.
        /// </summary>
        /// <param name="oClass">The Class to be used.</param>
        /// <param name="method">The method name to be executed.</param>
        /// <returns>Returns a ManagementBaseObject</returns>
        public ManagementBaseObject ExecuteMethod(ManagementClass oClass, string method)
        {
            if (oClass != null)
            {
                oClass.Get();
                ManagementBaseObject inParams = oClass.GetMethodParameters(method);
                return oClass.InvokeMethod(method, inParams, new InvokeMethodOptions());
            }
            else
            {
                return null;
            }
        }

        #endregion //ExecuteMethod

        #endregion //Public Methods
    }
}

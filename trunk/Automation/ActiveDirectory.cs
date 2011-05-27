//CCMManager
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
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;

namespace CCMManager.Automation
{
    public class ActiveDirectory : IDisposable
    {
        #region //Private Methods

        /// <summary>
        /// Find and Return the name of the closest Bridgehead Server for the Site.
        /// </summary>
        /// <returns>string - Name of bridgehead server.</returns>
        private string GetClosestGC()
        {
            //ActiveDirectorySite site = ActiveDirectorySite.GetComputerSite();
            //return site.BridgeheadServers[0].ToString();
            return ActiveDirectorySite.GetComputerSite().BridgeheadServers[0].ToString();
        }

        #endregion //Private Methods

        public List<string> FindMatchingComputers(string sFilterString)
        {
            if (!sFilterString.EndsWith("*") && !sFilterString.EndsWith("$") && !sFilterString.EndsWith("%"))
            {
                sFilterString = sFilterString += "$";
            }
            if (sFilterString.EndsWith("%"))
            {
                sFilterString = sFilterString.Replace('%', '*');
            }

            string filter = string.Format("(&(objectCategory=Computer)(sAMAccountName={0}))", sFilterString);
            List<string> Matches = new List<string>();

            DirectoryEntry de = new DirectoryEntry(string.Format("LDAP://{0}", this.GetClosestGC()));
            DirectorySearcher ds = new DirectorySearcher(de);
            SearchResultCollection results;
            try
            {
                ds.ReferralChasing = ReferralChasingOption.All;
                ds.SearchScope = SearchScope.Subtree;
                ds.PropertiesToLoad.Add("sAMAccountName");
                ds.Filter = filter;
                results = ds.FindAll();

                if (results.Count > 0)
                {
                    foreach (SearchResult sr in results)
                    {
                        Matches.Add(sr.Properties["sAMAccountName"][0].ToString().Replace("$", ""));
                    }
                }

                results.Dispose();
            }
            catch
            {
                //ERROR....
            }
            finally
            {
                de.Dispose();
                ds.Dispose();

            }
            return Matches;
        }

        #region Inherited Methods

        public void Dispose()
        {
            GC.Collect();
        }

        #endregion //Inherited Methods
    }
}

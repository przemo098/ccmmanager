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

using System.ComponentModel.Composition;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using Caliburn.Micro;
using CCMManager.Models;

namespace CCMManager.Services
{
    [Export(typeof(ActiveDirectory))]
    public class ActiveDirectory
    {

        public BindableCollection<IComputer> FindMatchingComputers(string filterName)
        {
            if (!filterName.EndsWith("*") && !filterName.EndsWith("$") && !filterName.EndsWith("%"))
            {
                filterName = filterName += "$";
            }
            if (filterName.EndsWith("%"))
            {
                filterName = filterName.Replace('%', '*');
            }

            string filter = string.Format("(&(objectCategory=Computer)(sAMAccountName={0}))", filterName);
            BindableCollection<IComputer> Matches = new BindableCollection<IComputer>();

            DirectoryEntry de = new DirectoryEntry(string.Format("LDAP://{0}",GetClosestDC()));
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
                        Computer c = new Computer() { Name = sr.Properties["sAMAccountName"][0].ToString().Replace("$", "") };
                        Matches.Add(c);
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

        private string GetClosestDC()
        {
            ActiveDirectorySite site = ActiveDirectorySite.GetComputerSite();
            return site.BridgeheadServers[0].ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CCMManager.Services
{
    using Caliburn.Micro;
    using System.ComponentModel.Composition;
    using System.DirectoryServices;
    using System.DirectoryServices.ActiveDirectory;
    using Models;

    [Export(typeof(ActiveDirectory))]
    public class ActiveDirectory
    {

        public BindableCollection<Computer> FindMatchingComputers(string filterName)
        {
            if (!filterName.EndsWith("*") && !filterName.EndsWith("$"))
            {
                filterName = filterName += "$";
            }
            string filter = string.Format("(&(objectCategory=Computer)(sAMAccountName={0}))", filterName);
            BindableCollection<Computer> Matches = new BindableCollection<Computer>();

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

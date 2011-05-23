namespace CCMManager.Models
{
    using System.Collections.Generic;

    public interface IComputer
    {
        string Name { get; set; }
        IList<string> MacAddresses { get; set; }
        IClassroom Parent { get; set; }

    }
}

namespace CCMManager.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Computer : IComputer
    {
        #region Properties

        public string Name { get; set; }
        public IList<string> MacAddresses { get; set; }
        public IClassroom Parent { get; set; }

        #endregion //Properties

        #region Constructors

        public Computer()
        {

        }

        public Computer(string name, IClassroom parent = null)
        {
            this.Name = name;
            this.Parent = parent;
            this.MacAddresses = new List<string>();
        }

        #endregion //Constructors


        
    }
}

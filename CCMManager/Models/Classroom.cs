namespace CCMManager.Models
{
    using System;
    using Caliburn.Micro;

    [Serializable]
    public class Classroom : IClassroom
    {
        #region Properties and Backing Fields

        public string Name { get; set; }

        public BindableCollection<IComputer> Computers
        {
            get;
            set;
        }

        #endregion //Properties and Backing Fields

        #region Constructors

        public Classroom()
        {
            Computers = new BindableCollection<IComputer>();
        }

        public Classroom(string name, BindableCollection<IComputer> computers = null)
        {
            this.Computers = new BindableCollection<IComputer>();
            this.Name = name;

        }

        #endregion
    }
}

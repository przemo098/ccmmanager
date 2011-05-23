namespace CCMManager.Models
{
    using System;
    using Caliburn.Micro;

    public class ClassroomActions : IClassroom
    {
        #region Properties and Backing Fields

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public BindableCollection<ActionsHomeModel> Computers
        {
            get;
            set;
        }

        #endregion //Properties and Backing Fields

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ClassroomActions()
        {
            Computers = new BindableCollection<ActionsHomeModel>();
        }

        /// <summary>
        /// Constructor with Properties
        /// </summary>
        /// <param name="name">The name of the Classroom.</param>
        /// <param name="computers">a BindableCollection<ActionsHomeModel> of Computer Objects</param>
        public ClassroomActions(string name, BindableCollection<IComputer> computers = null)
        {
            this.Name = name;
            if (computers != null)
            {
                Computers = new BindableCollection<ActionsHomeModel>();
                foreach (IComputer c in computers)
                {
                    Computers.Add(new ActionsHomeModel(c));
                }
            }
            else
            {
                Computers = new BindableCollection<ActionsHomeModel>();
            }
        }

        #endregion //Constructors


        BindableCollection<IComputer> IClassroom.Computers
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

namespace CCMManager.Models
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class ActionsHomeModel : PropertyChangedBase, IComputer
    {
        #region Properties and BackingFields

        private ComputerStates _status;
        public ComputerStates Status
        {
            get { return _status; }
            set { 
                _status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        private ImageSource _currentstatusimage;
        public ImageSource CurrentStatusImage
        {
            get { return _currentstatusimage; }
            set
            {
                _currentstatusimage = value;
                NotifyOfPropertyChange(() => CurrentStatusImage);
            }
        }

        private string _loggedonuser;
        public string LoggedOnUser
        {
            get { return _loggedonuser; }
            set
            {
                _loggedonuser = value;
                NotifyOfPropertyChange(() => LoggedOnUser);
            }
        }

        private string _actionStatus;
        public string ActionStatus
        {
            get { return _actionStatus; }
            set 
            {
                _actionStatus = value;
                NotifyOfPropertyChange(() => ActionStatus);
            } 
        }

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

        public IList<string> MacAddresses { get; set; }

        private IClassroom _parent;
        public IClassroom Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        /// <summary>
        /// Contains a list of RemoteAction objects to be performed on this host.
        /// </summary>
        private BindableCollection<RemoteAction> _actions;
        public BindableCollection<RemoteAction> Actions
        {
            get { return _actions; }
            set
            {
                _actions = value;
                NotifyOfPropertyChange(() => Actions);
            }
        }

        #endregion //Properties and BackingFields

        #region Constructors

        public ActionsHomeModel(IComputer computer)
        {
            // TODO: Complete member initialization
            this.Name = computer.Name;
            this.Parent = computer.Parent;
            this.Status = ComputerStates.Unknown;
            this.Actions = new BindableCollection<RemoteAction>();
            this.CurrentStatusImage = new BitmapImage(new Uri("pack://application:,,,/Images/system-help-3.png"));
            //this.ActionStatus = "Blank";
        }

        #endregion //Constructors
 
    }
}

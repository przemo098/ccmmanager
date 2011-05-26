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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;

namespace CCMManager.Models
{
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

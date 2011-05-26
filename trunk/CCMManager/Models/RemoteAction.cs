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

using System.Collections.Generic;
using System.Windows.Media;
using Caliburn.Micro;

namespace CCMManager.Models
{
    public abstract class RemoteAction : PropertyChangedBase
    {
        #region Properties and Backing Fields

        private RemoteActionState _state;
        public RemoteActionState State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyOfPropertyChange(() => State);
                NotifyOfPropertyChange(() => CurrentActionImage);
            }
        }

        private string _remoteActionName;
        public string RemoteActionName
        {
            get { return _remoteActionName; }
            set
            {
                _remoteActionName = value;
                NotifyOfPropertyChange(() => RemoteActionName);
            }
        }

        private Dictionary<RemoteActionState, ImageSource> _images;
        private Dictionary<RemoteActionState, ImageSource> Images
        {
            get { return _images; }
            set
            {
                _images = value;
                NotifyOfPropertyChange(() => Images);
            }
        }

        public ImageSource CurrentActionImage
        {
            get { return Images[State]; }
        }

        #endregion //Properties and Backing Fields

        #region Constructor

        public RemoteAction(Dictionary<RemoteActionState, ImageSource> img)
        {
            this.State = RemoteActionState.NotStarted;
            this.Images = img;
        }

        #endregion //Constructor

        #region Methods

        /// <summary>
        /// To be implemented on the individual Action.
        /// </summary>
        public abstract void Execute(object context = null);

        #endregion //Methods
    }
}

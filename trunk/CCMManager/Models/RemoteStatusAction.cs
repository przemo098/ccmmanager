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
    public class RemoteStatusAction : PropertyChangedBase
    {
        #region Properties and Backing Fields

        private ComputerStates _state;
        public ComputerStates State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyOfPropertyChange(() => State);
                //NotifyOfPropertyChange(() => CurrentActionImage);
            }
        }

        private Dictionary<ComputerStates, ImageSource> _images;
        private Dictionary<ComputerStates, ImageSource> Images
        {
            get { return _images; }
            set
            {
                _images = value;
                NotifyOfPropertyChange(() => Images);
            }
        }

        //public ImageSource CurrentStatusImage
        //{
        //    get { return Images[State]; }
        //}

        #endregion //Properties and Backing Fields

        #region Methods

        public ImageSource CurrentActionImage
        {
            get { return Images[State]; }
        }

        #endregion //Methods

        #region Constructors

        public RemoteStatusAction(Dictionary<ComputerStates, ImageSource> images = null)
        {
            this.State = ComputerStates.Unknown;
            this.Images = images;
            
            
        }

        #endregion //Constructors
    }
}

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
using Caliburn.Micro;

namespace CCMManager.Models
{
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

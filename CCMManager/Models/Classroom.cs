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

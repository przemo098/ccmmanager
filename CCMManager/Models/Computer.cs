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

namespace CCMManager.Models
{
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

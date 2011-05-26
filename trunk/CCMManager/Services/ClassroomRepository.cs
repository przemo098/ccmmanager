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
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Caliburn.Micro;
using CCMManager.Models;

namespace CCMManager.Services
{


    [Export(typeof(ClassroomRepository)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClassroomRepository
    {
        private readonly string _stateFile;
        private BindableCollection<IClassroom> _classroomStore;

        [ImportingConstructor]
        public ClassroomRepository()
        {
            this._stateFile = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "ClassroomManager.store");
            this.DeSerialize();
        }

        
        public BindableCollection<IClassroom> GetClassrooms()
        {
            return new BindableCollection<IClassroom>(this._classroomStore);
        }

        public void SetClassrooms(BindableCollection<IClassroom> classrooms)
        {
            this._classroomStore = classrooms;
            this.Serialize();
        }

        internal void AddClassroom(Classroom room)
        {
            this._classroomStore.Add(room);
            this.Serialize();
        }

        internal void DeleteClassroom(Classroom room)
        {
            this._classroomStore.Remove(room);
            this.Serialize();
        }


        /// <summary>
        /// Save the Repository to disk.
        /// </summary>
        private void Serialize()
        {
            using (FileStream stream =
                File.Open(this._stateFile, FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this._classroomStore);
            }
        }

        /// <summary>
        /// Open and read in the repository.
        /// </summary>
        private void DeSerialize()
        {
            if (File.Exists(_stateFile))
            {
                using (FileStream stream = File.Open(this._stateFile, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    this._classroomStore = (BindableCollection<IClassroom>)formatter.Deserialize(stream);
                }
            }
            else
            {
                this._classroomStore = new BindableCollection<IClassroom>();
            }
        }

    }
}

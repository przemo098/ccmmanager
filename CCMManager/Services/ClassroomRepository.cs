namespace CCMManager.Services
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Caliburn.Micro;
    using Models;
    using System.ComponentModel.Composition;

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

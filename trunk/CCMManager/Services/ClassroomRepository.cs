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
        private BindableCollection<Classroom> _classroomStore;

        [ImportingConstructor]
        public ClassroomRepository()
        {
            this._stateFile = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "ClassroomManager.store");
            this.DeSerialize();
        }

        
        public BindableCollection<Classroom> GetClassrooms()
        {
            return new BindableCollection<Classroom>(this._classroomStore);
        }

        public void SetClassrooms(BindableCollection<Classroom> classrooms)
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
                    this._classroomStore = (BindableCollection<Classroom>)formatter.Deserialize(stream);
                }
            }
            else
            {
                this._classroomStore = new BindableCollection<Classroom>();
            }
        }

    }
}

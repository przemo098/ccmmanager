using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCMManager.ViewModels
{
    using System.ComponentModel.Composition;
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Framework;
    using Models;
    using Services;
    using Resources;
    using System.Threading;
    using System.ComponentModel;
   

    /// <summary>
    /// Defines a view to display the Classroom Editor Dialog
    /// </summary>
    [Export(typeof(EditorViewModel))]
    public class EditorViewModel : Screen
    {
        [Import]
        private ClassroomRepository _repository;
        private ActiveDirectory _adFunctions;
        private BackgroundWorker _worker = new BackgroundWorker();
        
        [ImportingConstructor]
        public EditorViewModel(ClassroomRepository repository, ActiveDirectory adFunctions)
        {
            _repository = repository;
            _adFunctions = adFunctions;
            Classrooms = new BindableCollection<Classroom>(_repository.GetClassrooms());
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);

        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BindableCollection<Computer> results = (BindableCollection<Computer>)e.Result;
            if (results.Count > 0)
            {
                foreach (Computer c in results)
                {
                    if (!AlreadyExists(c))
                    {
                        SelectedClassroom.Computers.Add(c);
                        NotifyOfPropertyChange(() => Computers);
                    }
                }
            }
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _adFunctions.FindMatchingComputers(e.Argument.ToString());
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            //Classrooms = new BindableCollection<Classroom>(repository.GetClassrooms());
        }

        #region Properties & BackingFields

        private BindableCollection<Classroom> _classrooms;
        public BindableCollection<Classroom> Classrooms
        {
            get { return _classrooms; }
            set
            {
                _classrooms = value;
                NotifyOfPropertyChange(() => Classrooms);
            }
        }

        private Classroom _selectedClassroom;
        public Classroom SelectedClassroom
        {
            get { return _selectedClassroom; }
            set
            {
                _selectedClassroom = value;
                NotifyOfPropertyChange(() => SelectedClassroom);
                NotifyOfPropertyChange(() => Computers);
            }
        }

        //private BindableCollection<Computer> _comptuers;
        public BindableCollection<Computer> Computers
        {
            get 
            {
                if (SelectedClassroom != null)
                    return SelectedClassroom.Computers;
                else
                    return null;
            }
            set
            {
                SelectedClassroom.Computers = value;
                NotifyOfPropertyChange(() => Computers);
            }
        }

        private Computer _selectedComputer;
        public Computer SelectedComputer
        {
            get { return _selectedComputer; }
            set
            {
                _selectedComputer = value;
                NotifyOfPropertyChange(() => SelectedComputer);
            }
        }
        

        public string SingleComputerName { get; set; }

        public string ADComputerNameFilter { get; set; }


        #endregion //Properties & BackingFields

        #region Methods

        /// <summary>
        /// Display the Dialog to add a new Classroom.
        /// </summary>
        /// <param name="o"></param>
        public void NewRoom(object o)
        {
            
            AddClassroomDialog newRoomDlg = new AddClassroomDialog();

            newRoomDlg.Owner = App.Current.MainWindow;
            newRoomDlg.ShowDialog();

            if (newRoomDlg.DialogResult == true)
            {
                // Add the room
                Classroom c = new Classroom() { Name = newRoomDlg.ClassroomName.Text };
                if (!AlreadyExists(c))
                    Classrooms.Add(c); // Add it
                    SelectedClassroom = c; // Select the new room
            }
            
        }

        private bool AlreadyExists(Classroom c)
        {
            foreach (Classroom ec in Classrooms)
            {
                if (ec.Name == c.Name)
                    return true;
                    break;
            }
            return false;
        }

        private bool AlreadyExists(Computer c)
        {
            foreach (Computer ec in SelectedClassroom.Computers)
            {
                if (ec.Name == c.Name)
                    return true;
                    break;
            }
            return false;
        }

        /// <summary>
        /// Adds a Single Computer to the Classroom.
        /// </summary>
        /// <param name="o"></param>
        public void AddSingleComputer(object o)
        {
            Computer pc = new Computer(SingleComputerName, SelectedClassroom);
            if (!AlreadyExists(pc))
            {
                SelectedClassroom.Computers.Add(pc);
                NotifyOfPropertyChange(() => Computers);
            }
        }

        /// <summary>
        /// Adds a list of Computers to the Classroom, retreived from Active Directory.
        /// </summary>
        /// <param name="o"></param>
        public void AddADComputers(object o)
        {
            _worker.RunWorkerAsync(ADComputerNameFilter);
        }

        /// <summary>
        /// Removes the Selected Item(s) from the list of Computers in the Classroom.
        /// </summary>
        /// <param name="o"></param>
        public void RemoveSelected(object o)
        {
            SelectedClassroom.Computers.Remove(SelectedComputer);
        }

        /// <summary>
        /// Clears the list of Computers in the selected Classroom.
        /// </summary>
        /// <param name="o"></param>
        public void ClearList(object o)
        {
            SelectedClassroom.Computers.Clear();
        }

        /// <summary>
        /// Saves the current configuration to the repository, and loads the ActionsViewModel.
        /// </summary>
        /// <param name="o"></param>
        public void Save(object o)
        {
            _repository.SetClassrooms(Classrooms);
            var parent = Parent as ShellViewModel;

            parent.ActivateItem(parent.Items[0]);

        }

        #endregion //Methods
    }
}

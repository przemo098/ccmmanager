using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCMManager.ViewModels
{
    using System.ComponentModel.Composition;
    using Caliburn.Micro;
    using Framework;
    using Services;
    using Models;

    [Export(typeof(ActionsHomeViewModel))]
    [Export(typeof(IChildScreen<ActionsViewModel>))]
    public class ActionsHomeViewModel : Screen, IChildScreen<ActionsViewModel>
    {
        private ClassroomRepository _repository;

        [ImportingConstructor]
        public ActionsHomeViewModel(ClassroomRepository repository)
        {
            DisplayName = "Actions";
            _repository = repository;
            Classrooms = new BindableCollection<Classroom>(_repository.GetClassrooms());
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


        #endregion //Properties and BackingFields

        public string ScreenId
        {
            get { return GetType().Name; }
        }

        public int? Order 
        { 
            get { return 0; } 
        }

        public void EditClassrooms(object o)
        {
            //Load the EditorViewModel instead of this one...
            var myDirectParent = Parent as ActionsViewModel;
            var realParent = myDirectParent.Parent as ShellViewModel;

            realParent.ActivateItem(realParent.Items[1]);
            
        }

        public void ViewComputer(object sender)
        {
            Console.WriteLine("Hit");
        }
    }
}

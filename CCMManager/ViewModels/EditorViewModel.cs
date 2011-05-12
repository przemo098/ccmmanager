using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCMManager.ViewModels
{
    using System.ComponentModel.Composition;
    using Caliburn.Micro;
    using Framework;
    using Models;
    using Services;

    /// <summary>
    /// Defines a view to display the Classroom Editor Dialog
    /// </summary>
    [Export(typeof(EditorViewModel))]
    public class EditorViewModel : Screen
    {
        ClassroomRepository repository = new ClassroomRepository();
        
        [ImportingConstructor]
        public EditorViewModel()
        {
           //Classrooms = new BindableCollection<Classroom>(repository.GetClassrooms());
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
                NotifyOfPropertyChange(() => "Classrooms");
            }
        }

        #endregion //Properties & BackingFields
    }
}

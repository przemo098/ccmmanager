namespace CCMManager.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using CCMManager.Framework;
    using Caliburn.Micro;

    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell
    {

        [ImportingConstructor]
        public ShellViewModel(ActionsViewModel actionsViewModel,
            EditorViewModel EditorViewModel,
            IEventAggregator eventAggregator)
        {
            HasActiveDialog = false;
            eventAggregator.Subscribe(this);
            Items.Add(actionsViewModel);
            Items.Add(EditorViewModel);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            //DisplayName = AppStrings.AppTitle;
            ActivateItem(Items[0]);
        }

        public bool HasActiveDialog { get; set; }

    }
}

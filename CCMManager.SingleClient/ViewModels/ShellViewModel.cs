using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using CCMManager.SingleClient.Framework;

namespace CCMManager.SingleClient.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell
    {
        [ImportingConstructor]
        public ShellViewModel(ClientAgentViewModel clientAgents,
            ClientAdvertismentsViewModel clientAdvertisments,
            ClientProcessesViewModel clientProcesses,
            ClientInventoryViewModel clientInventories,
            IEventAggregator eventAggregator)
        {
            Items.Add(clientAgents);
            Items.Add(clientAdvertisments);
            Items.Add(clientProcesses);
            Items.Add(clientInventories);
            HasActiveDialog = false;
            eventAggregator.Subscribe(this);
            DisplayName = "SingleClient";
        }

        #region Protected Methods

        protected override void OnInitialize()
        {
            base.OnInitialize();
            //DisplayName = AppStrings.AppTitle;
            ActivateItem(Items[0]);
        }

        #endregion //Protected Methods

        #region Public Properties

        public bool HasActiveDialog { get; set; }

        #endregion //Public Properties
    }
}

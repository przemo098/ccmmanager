namespace CCMManager.Models
{
    using System.Collections.Generic;
    using System.Windows.Media;
    using Caliburn.Micro;

    public abstract class RemoteAction : PropertyChangedBase
    {
        #region Properties and Backing Fields

        private RemoteActionState _state;
        public RemoteActionState State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyOfPropertyChange(() => State);
                NotifyOfPropertyChange(() => CurrentActionImage);
            }
        }

        private Dictionary<RemoteActionState, ImageSource> _images;
        private Dictionary<RemoteActionState, ImageSource> Images
        {
            get { return _images; }
            set
            {
                _images = value;
                NotifyOfPropertyChange(() => Images);
            }
        }

        public ImageSource CurrentActionImage
        {
            get { return Images[State]; }
        }

        #endregion //Properties and Backing Fields

        #region Constructor

        public RemoteAction(Dictionary<RemoteActionState, ImageSource> img)
        {
            this.State = RemoteActionState.NotStarted;
            this.Images = img;
        }

        #endregion //Constructor

        #region Methods

        /// <summary>
        /// To be implemented on the individual Action.
        /// </summary>
        public abstract void Execute(object context = null);

        #endregion //Methods
    }
}

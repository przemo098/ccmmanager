namespace CCMManager.Models
{
    using System.Collections.Generic;
    using System.Windows.Media;
    using Caliburn.Micro;

    public class RemoteStatusAction : PropertyChangedBase
    {
        #region Properties and Backing Fields

        private ComputerStates _state;
        public ComputerStates State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyOfPropertyChange(() => State);
                NotifyOfPropertyChange(() => CurrentActionImage);
            }
        }

        private Dictionary<ComputerStates, ImageSource> _images;
        private Dictionary<ComputerStates, ImageSource> Images
        {
            get { return _images; }
            set
            {
                _images = value;
                NotifyOfPropertyChange(() => Images);
            }
        }

        public ImageSource CurrentStatusImage
        {
            get { return Images[State]; }
        }

        #endregion //Properties and Backing Fields

        #region Methods

        public ImageSource CurrentActionImage
        {
            get { return Images[State]; }
        }

        #endregion //Methods

        #region Constructors

        public RemoteStatusAction(Dictionary<ComputerStates, ImageSource> images)
        {
            this.State = ComputerStates.Unknown;
            this.Images = images;
        }

        #endregion //Constructors
    }
}

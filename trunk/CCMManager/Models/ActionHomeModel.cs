using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCMManager.Models
{
    public class ActionHomeModel : Computer
    {
        private string _status;
        public string Status
        {
            get;
            set;
        }

        private string _actionState;
        public string ActionState { get; set; }
    }
}

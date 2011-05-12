using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using Caliburn.Micro;

namespace CCMManager.Models
{
    public class Classroom
    {

        public BindableCollection<object> Computers
        {
            get;
            set;
        }

        public string Name { get; set; }
    }
}

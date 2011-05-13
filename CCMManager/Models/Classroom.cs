using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using Caliburn.Micro;


namespace CCMManager.Models
{
    [Serializable]
    public class Classroom
    {

        public Classroom()
        {
            Computers = new BindableCollection<Computer>();
        }

        public BindableCollection<Computer> Computers
        {
            get;
            set;
        }

        public string Name { get; set; }

        
    }
}

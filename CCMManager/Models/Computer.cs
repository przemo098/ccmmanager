using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCMManager.Models
{
    public class Computer
    {
        public string Name { get; set; }
        public string MACAddress { get; set; }
        public ComputerStates State { get; set; }
        public Classroom Parent { get; set; }

        public Computer()
        {

        }

        public Computer(string name)
        {
            this.Name = name;
        }

        public Computer(string name, Classroom parent)
        {
            this.Name = name;
            this.Parent = parent;
        }

        public Computer(string name, Classroom parent, string macAddress, ComputerStates state = ComputerStates.Unknown)
        {
            this.Name = name;
            this.Parent = parent;
            this.MACAddress = macAddress;
            this.State = state;
        }
    }
}

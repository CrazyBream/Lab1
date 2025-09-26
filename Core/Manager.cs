using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Manager : Person
    {
        public string Department { get; set; } 

        public Manager(string firstName, string lastName, string department = "General")
            : base(firstName, lastName)
        {
            Department = department;
        }
    }
}

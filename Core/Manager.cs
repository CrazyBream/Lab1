using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Manager : Person, IStudyable, IChessPlayer
    {
        public string Department { get; set; } 

        public Manager(string firstName, string lastName, string department = "General")
            : base(firstName, lastName)
        {
            Department = department;
        }

        public void Study()
        {
           
        }

        public void PlayChess()
        {
   
        }
    }
}

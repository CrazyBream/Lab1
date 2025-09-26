using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
        public class McdonaldsWorker : Person
        {
            public string Position { get; set; }

            public McdonaldsWorker(string firstName, string lastName, string position = "Employee")
                : base(firstName, lastName)
            {
                Position = position;
            }

        //public void Study() можна буде зробити щоб підвищувати кваліфікацію
        // {

        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class Person
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; } 

        protected Person(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("FirstName та LastName є обов'язковими.");
            }
            FirstName = firstName;
            LastName = lastName;
        }
    }
}

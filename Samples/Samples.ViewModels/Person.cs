using System;
using System.Collections.Generic;

namespace Samples.ViewModels
{
    public class Person
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] Addresses { get; set; }
        public List<string> Nicknames { get; set; }
        public DateTime DateOfBirth { get; set; }

        private int _someField;
        public int SomeProperty
        {
            get { return _someField; }
            set { _someField = value; }
        }
    }
}

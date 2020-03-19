using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnet.Asynchronous.App
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString() => string.Format("Id: {0}, Name: {1}, Age: {2}", Id, Name, Age);
    }
}

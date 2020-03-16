using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Asynchronous.App
{
    class Program
    {
        static void Main(string[] args)
        {
            ExamplePlinq();
        }

        private static void ExamplePlinq()
        {
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("Exemplo PLINQ");

            var customers = new List<Customer>() 
            {
                new Customer() { Id = 1, Name = "John", Age = 29 },
                new Customer() { Id = 2, Name = "Mark", Age = 13 },
                new Customer() { Id = 3, Name = "Edgar", Age = 25 },
            };

            var parallelSearch = from customer in customers.AsParallel()
                                 where customer.Age >= 18
                                 select customer;

            parallelSearch.ForAll((customer) => Console.WriteLine($"{customer.Id} - {customer.Name} - {customer.Age}"));

            Console.WriteLine("Exemplo PLINQ mantendo a ordenação da lista");
            parallelSearch = from customer in customers.AsParallel().AsOrdered()
                             where customer.Age >= 18
                             select customer;

            parallelSearch.ForAll((customer) => Console.WriteLine($"{customer.Id} - {customer.Name} - {customer.Age}"));
            
            Console.WriteLine("Combinando consultas paralelas e sequencias");
            var combineParallelSearch = (from customer in customers.AsParallel().AsOrdered()
                             where customer.Age >= 18
                             select customer).AsSequential().Take(1).ToList();

            combineParallelSearch.ForEach((customer) => Console.WriteLine($"{customer.Id} - {customer.Name} - {customer.Age}"));


            Console.WriteLine("-----------------------------------------------");
        }
    }
}

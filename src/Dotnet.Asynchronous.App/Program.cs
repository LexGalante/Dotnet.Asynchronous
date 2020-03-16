using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Asynchronous.App
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ExamplePlinq();
                Thread.Sleep(1000);
                ExampleSingleThread();
                Thread.Sleep(1000);
                ExampleBackgroundSingleThread();
                Thread.Sleep(1000);
                ExampleGetThreadResult();
                Thread.Sleep(1000);
                ExampleMultiTasks();
                Thread.Sleep(1000);
                ExampleCancelationToken();
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
            }
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
        
        private static void ExampleSingleThread()
        {
            var thread1 = new Thread(new ThreadStart(() => 
            {
                var nums = Enumerable.Range(0, 10);
                foreach (var num in nums)
                    Console.WriteLine($"thread1 - loop iteration - {num}");
            }));

            thread1.Start();
        }

        private static void ExampleBackgroundSingleThread()
        {
            var thread2 = new Thread(new ThreadStart(() =>
            {
                var nums = Enumerable.Range(0, 10);
                foreach (var num in nums)
                    Console.WriteLine($"thread2 - loop iteration - {num}");
            }));

            thread2.IsBackground = true;
            thread2.Start();
        }

        private static void ExampleGetThreadResult()
        {
            var task1 = Task.Run(() => 
            {
                var x = new Random().Next(1000, 9999);
                Console.WriteLine($"task1 - get random - {x}");
                Thread.Sleep(x);

                return x;
            });

            var result = task1.GetAwaiter().GetResult();
        }

        private static void ExampleMultiTasks()
        {
            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            var tasks = new Task[]
            {
                new Task(() => ExecuteTask(cancellationToken, 1), cancellationToken),
                new Task(() => ExecuteTask(cancellationToken, 2), cancellationToken),
                new Task(() => ExecuteTask(cancellationToken, 3), cancellationToken),
                new Task(() => ExecuteTask(cancellationToken, 4), cancellationToken),
                new Task(() => ExecuteTask(cancellationToken, 5), cancellationToken),
                new Task(() => ExecuteTask(cancellationToken, 6), cancellationToken)
            };

            foreach(var task in tasks)
            {
                task.Start();
                
                var sort = new Random().Next(0, 1);
                if (sort == 1)
                    source.Cancel();
            }
        }

        private static void ExampleCancelationToken()
        {
            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            var tasks = new Task[]
            {
                new Task(() => ExecuteTask(cancellationToken, 7), cancellationToken),
                new Task(() => ExecuteTask(cancellationToken, 8), cancellationToken),
                new Task(() => ExecuteTask(cancellationToken, 9), cancellationToken),
                new Task(() => ExecuteTask(cancellationToken, 10), cancellationToken),
                new Task(() => ExecuteTask(cancellationToken, 11), cancellationToken),
                new Task(() => ExecuteTask(cancellationToken, 12), cancellationToken)
            };

            foreach (var task in tasks)
                task.Start();

            source.Cancel();
        }

        private static void ExecuteTask(CancellationToken token, int number)
        {
            if (token.IsCancellationRequested)
                Console.WriteLine($"task{number} - cancelation token requested");
            else
            {
                Console.WriteLine($"task{number} - executed");
                Thread.Sleep((number * 1000));
            }
        }
    }
}

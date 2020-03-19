using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
                Console.WriteLine($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] Inicio programa");

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
                Thread.Sleep(1000);
                ExampleTaskContinueWith();
                Thread.Sleep(1000);
                ExampleParallelFor();
                Thread.Sleep(1000);
                ExampleThreadPool();
                Thread.Sleep(1000);
                ExampleKeywordAsyncAndAwait();
                Thread.Sleep(1000);
                ExampleThreadSafe();

                Console.WriteLine($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] Fim do programa");

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
            Console.WriteLine("----------------------------------------------------------------------------------------------");
            Console.WriteLine("Exemplo PLINQ");

            var customers = new List<Customer>() 
            {
                new Customer() { Id = 1, Name = "John", Age = 29 },
                new Customer() { Id = 2, Name = "Mark", Age = 13 },
                new Customer() { Id = 3, Name = "Edgar", Age = 25 },
            };

            var parallelSearch = from customer in customers.AsParallel()
                                                           .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                                                           .WithDegreeOfParallelism(2)
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


            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }
        
        private static void ExampleSingleThread()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------");
            Console.WriteLine("Exemplo SINGLE THREAD");

            var thread1 = new Thread(new ThreadStart(() => 
            {
                var nums = Enumerable.Range(0, 10);
                foreach (var num in nums)
                    Console.WriteLine($"thread1 - loop iteration - {num}");
            }));

            thread1.Start();

            Parallel.Invoke(() => Console.WriteLine("single thread"));

            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }

        private static void ExampleBackgroundSingleThread()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------");
            Console.WriteLine("Exemplo BACKGROUND SINGLE THREAD");

            var thread2 = new Thread(new ThreadStart(() =>
            {
                var nums = Enumerable.Range(0, 10);
                foreach (var num in nums)
                    Console.WriteLine($"thread2 - loop iteration - {num}");
            }));

            thread2.IsBackground = true;
            thread2.Start();

            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }

        private static void ExampleGetThreadResult()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------");
            Console.WriteLine("Exemplo GET THREAD RESULT");

            var task1 = Task.Run(() => 
            {
                var x = new Random().Next(1000, 9999);
                Console.WriteLine($"task1 - get random - {x}");
                Thread.Sleep(x);

                return x;
            });

            var result = task1.GetAwaiter().GetResult();
            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }

        private static void ExampleMultiTasks()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------");
            Console.WriteLine("Exemplo MULTI THREADS");

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
            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }

        private static void ExampleCancelationToken()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------");
            Console.WriteLine("Exemplo CANCELATION TOKEN");

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
            Console.WriteLine("----------------------------------------------------------------------------------------------");
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

        private static void ExampleTaskContinueWith()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------");
            Console.WriteLine("Exemplo CONTINUE WITH");

            var task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"taskContinueWith running...");
                Thread.Sleep(5000);
            });

            task.ContinueWith(task => 
            {
                Console.WriteLine($"taskContinueWith can be finished? {task.Status.ToString()}");
            });

            task.Wait();
            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }

        private static void ExampleParallelFor()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------");
            Console.WriteLine("Exemplo CONTINUE WITH");

            var json = File.ReadAllText("customers.json");
            var customers = JsonSerializer.Deserialize<List<Customer>>(json);
            Parallel.ForEach(customers, customer => Console.WriteLine($"Pararel iterarion"));

            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }

        private static void ExampleThreadPool()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------");
            Console.WriteLine("Exemplo THREAD POOL");

            ThreadPool.QueueUserWorkItem((object customer) =>
            {
                Console.WriteLine((customer as Customer).ToString());
            }, new Customer() { Id = 9999, Name = "Cledovaldo", Age = 2 });

            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }

        private static void ExampleKeywordAsyncAndAwait()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------");
            Console.WriteLine("Exemplo KEYWORDS ASYNC AWAIT");

            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            var task = GetCustomersAsync(cancellationToken);
            var customers = task.GetAwaiter().GetResult();
            Console.WriteLine($"GetCustomersAsync.Count = {customers.Count}");

            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }

        private static async Task<List<Customer>> GetCustomersAsync(CancellationToken cancellationToken)
        {
            using var stream = new StreamReader("customers.json");

            return await JsonSerializer.DeserializeAsync<List<Customer>>(stream.BaseStream);
        }

        private static void ExampleThreadSafe()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------");
            Console.WriteLine("Exemplo THREAD SAFE");
            
            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            var customers = GetCustomersAsync(cancellationToken).GetAwaiter().GetResult();
            var queue = new ConcurrentQueue<Customer>();
            Console.WriteLine("add customer in concurrent");
            Parallel.Invoke(
                () => customers.ForEach((customer) => queue.Enqueue(customer)),
                () => customers.ForEach((customer) => queue.Enqueue(customer)),
                () => customers.ForEach((customer) => queue.Enqueue(customer)),
                () => customers.ForEach((customer) => queue.Enqueue(customer))
            );
            if (queue.TryPeek(out Customer customer))
                Console.WriteLine($"concurrent queue peek: {customer.ToString()}");
            Console.WriteLine($"finished invoke customers in concurrent queue total: {queue.Count}");

            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }
    }
}

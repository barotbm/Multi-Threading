using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello");  
            int maxNumber = 100;

            var name = KLASingleton.Instance.Name;
            Console.WriteLine(name);
            Console.ReadLine();

            var task = RunThreads(maxNumber);
            // wait till RunThreads finish
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            Console.Read();
        }

        /// <summary>
        /// Run multiple threads synchronously 
        /// Pass the output of one thread to other
        /// Handle task cancellation
        /// </summary>
        /// <param name="maxNumber"></param>
        /// <returns></returns>
        static async Task RunThreads(int maxNumber)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            for (int i = 1; i <= maxNumber; i++)
            {
                if (i % 25 == 0)
                    tokenSource.Cancel();

                if (!token.IsCancellationRequested)
                {
                    await Task.Run(() =>
                    {
                        Print("Thread 1: ", i);
                        return i + 10;
                    }, token).ContinueWith(task =>
                    {
                        Print("Thread 2: ", task.Result);
                    }, token);

                    //await Task.Run(() =>
                    //{
                    //    Print("Thread 2: ", i);
                    //})/*;*/
                }
                else
                {
                    Console.WriteLine("Terminated!! Gracefully!");
                    break;
                }
            }
        }

        static void Print(string threadIdentifier, int input)
        {
            Console.WriteLine(threadIdentifier + input.ToString());
        }
    }

    public class KLASingleton
    {
        private static readonly object _lock = new Object();
        private static KLASingleton _instance;
        public static KLASingleton Instance
        {
            get
            {
                lock (_lock)
                {
                    if(_instance == null)
                        _instance = new KLASingleton();

                    return _instance;
                }
            }
        }

        public string Name { get; set; } = "KLA Example";
        private KLASingleton()
        {
           
        }       
    }
}
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Numerics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace MultiThreading
{
    internal class ArrayProcessor
    {
        private readonly object sync = new object();
        private int[] array;

        public ArrayProcessor(int size)
        {
            array = new int[size];
        }

        public void GenerateRandomArray()
        {
            lock (sync)
            {
                var stopwatch = Stopwatch.StartNew();

                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = new Random().Next();
                }

                stopwatch.Stop();
                Console.WriteLine($"Random Array in {stopwatch.Elapsed}");
            }
        }

        public void FindMin()
        {
            lock (sync)
            {
                var stopwatch = Stopwatch.StartNew();

                int min = array.Min();

                stopwatch.Stop();
                Console.WriteLine($"Min in Array: {min} in {stopwatch.Elapsed}");
            }
        }

        public void FindMax()
        {
            lock (sync)
            {
                var stopwatch = Stopwatch.StartNew();

                int max = array.Max();

                stopwatch.Stop();
                Console.WriteLine($"Max in Array: {max} in {stopwatch.Elapsed}");
            }
        }

        public void CalculateSum()
        {
            lock (sync)
            {
                var stopwatch = Stopwatch.StartNew();

                BigInteger sum = 0;

                foreach (var element in array)
                {
                    sum += element;
                }

                stopwatch.Stop();
                Console.WriteLine($"Sum of Array: {sum} in {stopwatch.Elapsed}");
            }
        }

        public void CalculateAverage()
        {
            lock (sync)
            {
                var stopwatch = Stopwatch.StartNew();

                double average = array.Average();

                stopwatch.Stop();
                Console.WriteLine($"Average of Array: {average} in {stopwatch.Elapsed}");
            }
        }

        public void CopyArrayPart()
        {
            lock (sync)
            {
                var stopwatch = Stopwatch.StartNew();

                int[] copy = new int[array.Length / 2];
                Array.Copy(array, copy, copy.Length);

                stopwatch.Stop();
                Console.WriteLine($"Copied Array Part in {stopwatch.Elapsed}");
            }
        }

        public void CharacterFrequency(string text)
        {
            lock (sync)
            {
                var stopwatch = Stopwatch.StartNew();

                var charFrequency = text.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

                stopwatch.Stop();

                Console.WriteLine($"Character Frequency in {stopwatch.Elapsed}:");
                foreach (var kvp in charFrequency)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }

        public void WordFrequency(string text)
        {
            lock (sync)
            {
                var stopwatch = Stopwatch.StartNew();

                var wordFrequency = text.Split(new[] { ' ', '\n', '\r', '\t', '.', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .GroupBy(word => word)
                                        .ToDictionary(g => g.Key, g => g.Count());

                stopwatch.Stop();

                Console.WriteLine($"Word Frequency in {stopwatch.Elapsed}:");
                foreach (var kvp in wordFrequency)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }

        public void ProcessTasks(int threadCount)
        {
            Task[] tasks = new Task[]
            {
                    Task.Run(() => GenerateRandomArray()),
                    Task.Run(() => FindMin()),
                    Task.Run(() => FindMax()),
                    Task.Run(() => CalculateSum()),
                    Task.Run(() => CalculateAverage()),
                    Task.Run(() => CopyArrayPart()),
                    Task.Run(() => CharacterFrequency("Create a program that can perform the following tasks in parallel")),
                    Task.Run(() => WordFrequency("Create a program that can perform the following tasks in parallel")),
            };
           
            Task.WaitAll(tasks);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the number of threads: ");
            int threadCount = int.Parse(Console.ReadLine());

            RunTasks(threadCount);

            Console.WriteLine($"Tasks completed");
        }

        static void RunTasks(int threadCount)
        {
            int[] arraySizes = { 1_000, 10_000, 100_000, 1_000_000 };

            foreach (int size in arraySizes)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                System.Threading.Thread.Sleep(2000);

                var arrProcessor = new ArrayProcessor(size);

                //Процессор	Intel(R) Pentium(R) CPU 2020M @ 2.40GHz, 2400 МГц, ядер: 2; 6 ГБ; Win 10

                //Для обсягу даних 1_000 найефективніший результат досягається при використанні 8 потоків 00:00:00.0488662
                //Для обсягу даних 10_000 - 4 потоків,  00:00:00.0398633.
                //Для обсягу даних 100_000 - 32 потоків,  00:00:00.1100027.
                //Для обсягу даних 1_000_000 - 16 потоків,  00:00:00.8783500.

                var sw = Stopwatch.StartNew();

                arrProcessor.ProcessTasks(threadCount);

                Console.WriteLine($"Array Size: {size}, Total Execution Time: {sw.Elapsed}");
            }
        }
    }
}

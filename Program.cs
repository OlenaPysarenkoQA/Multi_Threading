using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Numerics;
using System.Diagnostics.Metrics;

namespace MultiThreading
{
    internal class ArrayProcessor
    {
        private readonly object sync = new object();
        private int[] array;

        private Thread[] threads;

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

        public void ProcessThreads(int threadCount)
        {
            threads = new Thread[threadCount];

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ThreadProc) { IsBackground = true };
                threads[i].Start(i);
            }

            foreach (var thread in threads) thread.Join();
        }

        private void ThreadProc(object? state)
        {
            var length = array.Length;
            var index = (int)state;
            var count = length / threads.Length;

            var span = index == threads.Length - 1
                ? array.AsSpan((index * count)..)
                : array.AsSpan((index * count)..((index + 1) * count));

            for (var i = 0; i < span.Length; i++)
            {
                span[i] = new Random().Next();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the number of threads: ");
            int threadCount = int.Parse(Console.ReadLine());

            var arrProcessor = new ArrayProcessor(1_000);

            //Enter the number of threads: 1, [1_000]   //Enter the number of threads: 1, [1_000_000] 
            //1 00:00:00.0014091                        //1 00:00:00.7271688
            //2 00:00:00.0138184                        //2 00:00:00.0205835
            //3 00:00:00.0009145                        //3 00:00:00.0090088
            //4 00:00:00.0133612                        //4 00:00:00.1087749
            //5 00:00:00.0000550                        //5 00:00:00.0071860
            //6 00:00:00.0000021                        //6 00:00:00.0022331
            //7 00:00:00.0324787                        //7 00:00:00.0094106
            //8 00:00:00.0142031                        //8 00:00:00.0035241

            //Enter the number of threads: 2, [1_000]   //Enter the number of threads: 2, [1_000_000]
            //1 00:00:00.0006838                        //1 00:00:00.7499104
            //2 00:00:00.0089563                        //2 00:00:00.0135662
            //3 00:00:00.0000759                        //3 00:00:00.0166724
            //4 00:00:00.0004776 -opt2                  //4 00:00:00.0895216 - opt1
            //5 00:00:00.0000279 -opt1                  //5 00:00:00.0078364
            //6 00:00:00.0000013                        //6 00:00:00.0014413
            //7 00:00:00.0100541                        //7 00:00:00.0100911
            //8 00:00:00.0045502                        //8 00:00:00.0041528

            //Enter the number of threads: 3, [1_000]   //Enter the number of threads: 3, [1_000_000]
            //1 00:00:00.0006890                        //1 00:00:00.6405113 -opt1
            //2 00:00:00.0010361 -opt2                  //2 00:00:00.0087673
            //3 00:00:00.0000702                        //3 00:00:00.0085471 -opt1
            //4 00:00:00.0005044                        //4 00:00:00.0909434
            //5 00:00:00.0000400                        //5 00:00:00.0072811
            //6 00:00:00.0000008 -opt2                  //6 00:00:00.0014337
            //7 00:00:00.0098775 -opt2                  //7 00:00:00.0099720
            //8 00:00:00.0046558                        //8 00:00:00.0048487

            //Enter the number of threads: 4, [1_000]   //Enter the number of threads: 4, [1_000_000]
            //1 00:00:00.0006164 -opt2                  //1 00:00:00.7259865
            //2 00:00:00.0011816                        //2 00:00:00.0124596
            //3 00:00:00.0000579 -opt2                  //3 00:00:00.0099336
            //4 00:00:00.0005356                        //4 00:00:00.0923654
            //5 00:00:00.0000349 -opt2                  //5 00:00:00.0072531
            //6 00:00:00.0000007 -opt1                  //6 00:00:00.0014693
            //7 00:00:00.0094805 -opt1                  //7 00:00:00.0101828
            //8 00:00:00.0041058 -opt2                  //8 00:00:00.0042779

            //Enter the number of threads: 8, [1_000]   //Enter the number of threads: 8, [1_000_000]
            //1 00:00:00.0037507                        //1 00:00:01.2508314
            //2 00:00:00.0010722                        //2 00:00:00.0089598
            //3 00:00:00.0000706                        //3 00:00:00.0094447
            //4 00:00:00.0004756 -opt1                  //4 00:00:00.0993272
            //5 00:00:00.0000572                        //5 00:00:00.0072620
            //6 00:00:00.0000013                        //6 00:00:00.0014519
            //7 00:00:00.0262161                        //7 00:00:00.0105471
            //8 00:00:00.0040269 -opt1                  //8 00:00:00.0039106

            //Enter the number of threads: 16, [1_000]  //Enter the number of threads: 16, [1_000_000]
            //1 00:00:00.0007073                        //1 00:00:00.8147877
            //2 00:00:00.0010357 - opt1                 //2 00:00:00.0087197 -opt1
            //3 00:00:00.0000554 - opt1                 //3 00:00:00.0132737
            //4 00:00:00.0023734                        //4 00:00:00.0929432
            //5 00:00:00.0000367                        //5 00:00:00.0071814 -opt1
            //6 00:00:00.0000014                        //6 00:00:00.0013771 -opt1
            //7 00:00:00.0109813                        //7 00:00:00.0089056 -opt1
            //8 00:00:00.0043250                        //8 00:00:00.0034557 -opt2

            //Enter the number of threads: 32, [1_000]  //Enter the number of threads: 32, [1_000_000]
            //1 00:00:00.0006118  -opt1                 //1 00:00:00.7205086
            //2 00:00:00.0033064                        //2 00:00:00.0097110
            //3 00:00:00.0002161                        //3 00:00:00.0088938
            //4 00:00:00.0005434                        //4 00:00:00.0909297
            //5 00:00:00.0001994                        //5 00:00:00.0075167
            //6 00:00:00.0000009                        //6 00:00:00.0014171
            //7 00:00:00.0120990                        //7 00:00:00.0092767
            //8 00:00:00.0058390                        //8 00:00:00.0033990 -opt1

            var sw = Stopwatch.StartNew();

            arrProcessor.GenerateRandomArray();

            arrProcessor.FindMin();

            arrProcessor.FindMax();

            arrProcessor.CalculateSum();

            arrProcessor.CalculateAverage();

            arrProcessor.CopyArrayPart();

            var text = "Create a program that can perform the following tasks in parallel";
            arrProcessor.CharacterFrequency(text);

            arrProcessor.WordFrequency(text);

            arrProcessor.ProcessThreads(threadCount);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DiningPhilosophers
{
    class Philosophers
    {
        private const int PHILOSOPHER_COUNT = 5;

        static void Main(string[] args)
        {
            Stopwatch timePerParse1;
            long ticksThisTime = 0;
            timePerParse1 = Stopwatch.StartNew();

            // init Forks
            var Forks = new List<Fork>(PHILOSOPHER_COUNT);

            for (int i = 0; i < PHILOSOPHER_COUNT; ++i)
            {
                Forks.Add(new Fork(i));
            }

            // init philosophers
            // first philosopher
            Task[] tasks = new Task[PHILOSOPHER_COUNT];
            tasks[0] = new Task(() => Philoshoper.Eat(Forks[0], Forks[PHILOSOPHER_COUNT - 1], 1, 1, PHILOSOPHER_COUNT));

            // other philosophers
            for (int i = 1; i < PHILOSOPHER_COUNT; ++i)
            {
                int ix = i;
                tasks[ix] = new Task(() => Philoshoper.Eat(Forks[ix - 1], Forks[ix], ix + 1, ix, ix + 1));
            }

            // dinner starts
            Console.WriteLine("Dinner starts!");

            Parallel.ForEach(tasks, t =>
            {
                t.Start();
            });

            // Wait until all philosophers finished
            Task.WaitAll(tasks);

            timePerParse1.Stop();
            ticksThisTime = timePerParse1.ElapsedTicks;
            Console.WriteLine("Program ran " + ticksThisTime + "ms");

            Console.WriteLine("Dinner done!");
            Console.ReadLine();
        }
    }

    public class Philoshoper
    {
        static public void Eat(Fork leftFork, Fork rightFork, int philosopherNumber, int leftForkNumber, int rightForkNumber)
        {
            while (true)
            {
                Stopwatch timePerParse2;
                long ticksThisTime = 0;

                // philosopher thinks
                Random r = new Random();
                int thinking_time = r.Next(0, 5000);
                Console.WriteLine("Philosopher {0} thinks.", philosopherNumber);
                Thread.Sleep(thinking_time);

                timePerParse2 = Stopwatch.StartNew();
                Console.WriteLine("Philosopher {0} wants to take Forks.", philosopherNumber);

                Fork first = leftFork;
                Fork second = rightFork;
                int firstFork = leftForkNumber;
                int secondFork = rightForkNumber;

                int even = philosopherNumber % 2;

                // switch forks
                if (even == 0)
                {
                    first = rightFork;
                    firstFork = rightForkNumber;
                    second = leftFork;
                    secondFork = leftForkNumber;
                }

                lock (first)
                {
                    timePerParse2.Stop();
                    ticksThisTime = timePerParse2.ElapsedTicks;
                    Console.WriteLine("Philosopher {0} was waiting for " + ticksThisTime + "ms", philosopherNumber);

                    Console.WriteLine("Philosopher {0} picked {1} Fork.", philosopherNumber, firstFork);

                    lock (second)
                    {
                        // philosopher eats
                        Console.WriteLine("Philosopher {0} picked {1} Fork.", philosopherNumber, secondFork);
                        Console.WriteLine("Philosopher {0} eats.", philosopherNumber);

                        int eating_time = r.Next(0, 10000);
                        Thread.Sleep(eating_time);
                        Console.WriteLine("Philosopher {0} stops eating.", philosopherNumber);
                    }
                    Console.WriteLine("Philosopher {0} released {1} Fork.", philosopherNumber, secondFork);

                }
                Console.WriteLine("Philosopher {0} released {1} Fork.", philosopherNumber, firstFork);
            }
        }
    }

    public class Fork
    {
        public Fork(int index)
        {
            this.ID = index;
        }

        public int ID { get; }
    }
}

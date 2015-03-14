using System;
using System.Diagnostics;
using System.IO;
using JapaneseCrossword.CrosswordSolvers;

namespace JapaneseCrossword
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            ICrosswordSolver solver = new MultithreadedCrosswordSolver();
            var status = solver.Solve(@"TestFiles\Winter.txt", "output1.txt");
            stopWatch.Stop();
            Console.WriteLine("  One threaded solver time: {0,3}   Status: {1}", stopWatch.Elapsed.Milliseconds, status);
            stopWatch = new Stopwatch();
            stopWatch.Start();
            solver = new CrosswordSolver();
            status = solver.Solve(@"TestFiles\Winter.txt", "output2.txt");
            stopWatch.Stop();
            Console.WriteLine("Multi threaded solver time: {0,3}   Status: {1}", stopWatch.Elapsed.Milliseconds, status);
            Console.WriteLine("\nOutput files contents equals?  {0}", File.ReadAllText("output1.txt").Equals(File.ReadAllText("output2.txt")));
            Console.ReadKey();
        }
    }
}

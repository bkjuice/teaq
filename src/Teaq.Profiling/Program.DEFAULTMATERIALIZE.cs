#if DEFAULTMATERIALIZE
using System;
using System.Diagnostics;
using Teaq.Profiling.Scenarios;

namespace Teaq.Profiling
{
    class Program
    {
#if INSTR
        private const int iterations = 2000;
#else
        private const int iterations = 20000;
#endif
        static void Main(string[] args)
        {
#if !INSTR
            var stopwatch = Stopwatch.StartNew();
#endif
            var scenario = new DefaultObjectMaterialization();
            var count = 0;
            for (int i = 0; i < iterations; ++i)
            {
                if (scenario.Run()) count++;
            }

            Console.WriteLine(count);
#if !INSTR
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
#endif
        }
    }
}
#endif
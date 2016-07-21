#if FASTTIME
using System;
using System.Diagnostics;
using Teaq.Profiling.Scenarios;

namespace Teaq.Profiling
{
    class Program
    {
        private const int iterations = 1000000;

        static void Main(string[] args)
        {
            // Profiling with instrumentation caused FastReflection to lose bad because of 
            // instrumentation overhead...this is a sanity check, which turns out well, about 40% better execution time
            // than FastMember, for this case:
            var scenario = new FastReflectionTimings();
            long fmResult;
            var frResult = scenario.RunFMFirst(iterations, out fmResult);
            Console.WriteLine("Fast Member first:");
            Console.WriteLine($"Fast Reflection: {frResult}ms <> Fast Member: {fmResult}ms ");
            GC.Collect();
            frResult = scenario.RunFRFirst(iterations, out fmResult);
            Console.WriteLine("Fast Reflection first:");
            Console.WriteLine($"Fast Reflection: {frResult}ms <> Fast Member: {fmResult}ms ");
            Console.ReadLine();
        }
    }
}
#endif
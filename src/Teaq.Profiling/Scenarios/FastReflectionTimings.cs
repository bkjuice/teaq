using System.Diagnostics;
using System.Reflection;
using FastMember;
using Teaq.FastReflection;

namespace Teaq.Profiling.Scenarios
{
    internal class FastReflectionTimings
    {
        private static readonly TypeDescription target = typeof(Target).GetTypeDescription(MemberTypes.Property);

        private static readonly PropertyDescription targetId = target.GetProperty("Id");

        private static readonly PropertyDescription targetValue = target.GetProperty("Value");

        private static readonly PropertyDescription targetSubTarget = target.GetProperty("SubTarget");

        private static readonly TypeAccessor targetA = TypeAccessor.Create(typeof(Target));

        public long RunFMFirst(int iterations, out long otherTimings)
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                this.SetUsingFastMember(1, "MEMB", new SubTarget());
            }

            stopwatch.Stop();
            otherTimings = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            for (int i = 0; i < iterations; ++i)
            {
                this.SetUsingFastReflection(1, "FAST", new SubTarget());
            }

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public long RunFRFirst(int iterations, out long otherTimings)
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                this.SetUsingFastReflection(1, "FAST", new SubTarget());
            }

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            for (int i = 0; i < iterations; ++i)
            {
                this.SetUsingFastMember(1, "MEMB", new SubTarget());
            }

            stopwatch.Stop();
            otherTimings = stopwatch.ElapsedMilliseconds;

            return time;
        }

        private object SetUsingFastReflection(object id, object value, object subTarget)
        {
        var instance = target.CreateInstance();
            targetId.SetValue(instance, id);
            targetValue.SetValue(instance, value);
            targetSubTarget.SetValue(instance, subTarget);
            return instance;
        }

        private object SetUsingFastMember(object id, object value, object subTarget)
        {
            var instance = targetA.CreateNew();
            var accessor = ObjectAccessor.Create(instance);
            accessor["Id"] = id;
            accessor["Value"] = value;
            accessor["SubTarget"] = subTarget;
            return instance;
        }

        private object SetUsingDynamic(object id, object value, object subTarget)
        {
            dynamic instance = target.CreateInstance();
            instance.Id = (int)id;
            instance.Value = (string)value;
            instance.SubTarget = (SubTarget)subTarget;
            return instance;
        }

        public class Target
        {
            public int Id { get; set; }

            public string Value { get; set; }

            public SubTarget SubTarget { get; set; }
        }

        public class SubTarget
        {
            public int Id { get; set; }
        }
    }
}

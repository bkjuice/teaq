using System.Reflection;
using FastMember;
using Teaq.FastReflection;

namespace Teaq.Profiling.Scenarios
{
    internal class FastReflectionVsDynamic
    {
        private static readonly TypeDescription target = typeof(Target).GetTypeDescription(MemberTypes.Property);

        private static readonly PropertyDescription targetId = target.GetProperty("Id");

        private static readonly PropertyDescription targetValue = target.GetProperty("Value");

        private static readonly PropertyDescription targetSubTarget = target.GetProperty("SubTarget");

        private static readonly TypeAccessor targetA = TypeAccessor.Create(typeof(Target));

        public bool Run()
        {
            return
                this.SetUsingFastReflection(1, "FAST", new SubTarget()) != null &&
                this.SetUsingFastMember(1, "MEMB", new SubTarget()) != null &&
                this.SetUsingDynamic(1, "DYNA", new SubTarget()) != null;
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

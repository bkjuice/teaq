using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Teaq;
using Teaq.FastReflection;
using Teaq.Tests.FastReflection.SupportingTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests
{
    [TestClass]
    public class PerformanceTests
    {
        // Use this constant when running unit tests to minimize processing:
        private const int maxLoops = 1;

        // Use this constant for running the load test to minimize measurment of test overhead.
        // private const int maxLoops = 10000;

        private static readonly PerformanceTarget fixture = PerformanceTarget.Build();

        private static readonly string[] propertyNames = typeof(PerformanceTarget).GetProperties().Select(prop => prop.Name).ToArray();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var targetType = typeof(PerformanceTarget).GetTypeDescription(MemberTypes.Property);
        }

        [TestMethod]
        public void Reflect_get_create_set()
        {
            var targetType = typeof(PerformanceTarget);
            for (int i = 0; i < maxLoops; i++)
            {
                var copy = Activator.CreateInstance(targetType);
                var properties = targetType.GetProperties();
                for (int x = 0; x < properties.Length; x++)
                {
                    var value = properties[x].GetValue(fixture, null);
                    properties[x].SetValue(copy, value, null);
                }
            }
        }

        [TestMethod]
        public void Reflect_get_create_set_by_name()
        {
            var targetType = typeof(PerformanceTarget);
            for (int i = 0; i < maxLoops; i++)
            {
                var copy = Activator.CreateInstance(targetType);
                for (int x = 0; x < propertyNames.Length; x++)
                {
                    var property = targetType.GetProperty(propertyNames[x]);
                    var value = property.GetValue(fixture, null);
                    property.SetValue(copy, value, null);
                }
            }
        }

        [TestMethod]
        public void Reflect_IL_get_create_set()
        {
            var targetType = typeof(PerformanceTarget).GetTypeDescription(MemberTypes.Property);
            for (int i = 0; i < maxLoops; i++)
            {
                var copy = targetType.CreateInstance();
                var properties = targetType.GetProperties();
                for(int x = 0; x < properties.Length; x++)
                {
                    var value = properties[x].GetValue(fixture);
                    properties[x].SetValue(copy, value);
                }
            }
        }

        [TestMethod]
        public void Reflect_IL_get_create_set_by_name()
        {
            var targetType = typeof(PerformanceTarget).GetTypeDescription(MemberTypes.Property);

            for (int i = 0; i < maxLoops; i++)
            {
                var copy = targetType.CreateInstance();
                for (int x = 0; x < propertyNames.Length; x++)
                {
                    var property = targetType.GetProperty(propertyNames[x]);
                    var value = property.GetValue(fixture);
                    property.SetValue(copy, value);
                }
            }
        }

        [TestMethod]
        public void Performance_Direct_copy()
        {
            for (int i = 0; i < maxLoops; i++)
            {
                var copy = new PerformanceTarget();
                copy.Id1 = fixture.Id1;
                copy.Id2 = fixture.Id2;
                copy.Id3 = fixture.Id3;
                copy.Id4 = fixture.Id4;
                copy.Value1 = fixture.Value1;
                copy.Value2 = fixture.Value2;
                copy.Value3 = fixture.Value3;
                copy.Value4 = fixture.Value4;
                copy.Value5 = fixture.Value5;
                copy.Value6 = fixture.Value6;
                copy.Value7 = fixture.Value7;
                copy.Value8 = fixture.Value8;
                copy.Value9 = fixture.Value9;
                copy.Value10 = fixture.Value10;
                copy.Value11 = fixture.Value11;
                copy.Value12 = fixture.Value12;
                copy.Value13 = fixture.Value13;
                copy.Value14 = fixture.Value14;
                copy.Value15 = fixture.Value15;
                copy.Value16 = fixture.Value16;
                copy.Value17 = fixture.Value17;
                copy.Value18 = fixture.Value18;
                copy.Value19 = fixture.Value19;
            }
        }
    }
}

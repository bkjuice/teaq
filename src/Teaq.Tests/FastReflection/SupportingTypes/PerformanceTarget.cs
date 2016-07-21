using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teaq.Tests.FastReflection.SupportingTypes
{
    public class PerformanceTarget
    {
        public int Id1 { get; set; }
        public long Id2 { get; set; }
        public short Id3 { get; set; }
        public double Id4 { get; set; }

        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
        public string Value5 { get; set; }
        public string Value6 { get; set; }
        public string Value7 { get; set; }
        public string Value8 { get; set; }
        public string Value9 { get; set; }
        public string Value10 { get; set; }

        public string Value11 { get; set; }
        public string Value12 { get; set; }
        public string Value13 { get; set; }
        public string Value14 { get; set; }
        public string Value15 { get; set; }
        public string Value16 { get; set; }
        public string Value17 { get; set; }
        public string Value18 { get; set; }
        public string Value19 { get; set; }

        public static PerformanceTarget Build()
        {
            return new PerformanceTarget
            {
                Id1 = 1,
                Id2 = long.MaxValue,
                Id3 = short.MinValue,
                Id4 = double.MaxValue,

                Value1 = "Value1",
                Value2 = "Value22",
                Value3 = "Value333",
                Value4 = "Value4444",
                Value5 = "Value55555",
                Value6 = "Value666666",
                Value7 = "Value7777777",
                Value8 = "Value88888888",
                Value9 = "Value999999999",
                Value10 = "Value1010101010",
                Value11 = "Value1",
                Value12 = "Value22",
                Value13 = "Value333",
                Value14 = "Value4444",
                Value15 = "Value55555",
                Value16 = "Value666666",
                Value17 = "Value7777777",
                Value18 = "Value88888888",
                Value19 = "Value999999999",
            };
        }
    }
}

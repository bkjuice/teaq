using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teaq.FastReflection.MapperGeneration
{
    internal class MemberMapping
    {
        public Type Type { get; set; }

        public int Ordinal { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"instance.{this.Name } = reader[{this.Ordinal}]";
        }
    }
}

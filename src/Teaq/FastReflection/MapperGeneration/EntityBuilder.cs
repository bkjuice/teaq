using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teaq.FastReflection.MapperGeneration
{
    internal class EntityBuilder
    {
        /*
         * The mapper must be reflected upon entity introduction.
         * The mapper must test for null.
         * The mapper must test for the reader's field names...it may not be able to know.
         * 
         * Could emit a "typed reader" that looks like active record?
         * MoveNext = Reader.Read()
         * myCustomer.Id =  myCustomerReader.Id?
         * meh
         * 
         * If I pass an interface as an abstract mapping, this could work. Can materialize underneath. No methods would be supported.
         * Read only properties would be fine.
         * ReadAbstractEntities<ICustomer>();
         * 
         * --- Interesting approach.
         * 
         * What about a sequential grouping of property accessors, with functions that read?
         *  - Cache the ordinal, the conversion, the property, and loop...
         *  - let the fluent interface allow for direct mapping definition?
         *  .WithMapping(r => {});
         *  
         *  Can dynamically generate mappings also, by default.
         *  
         *  Read once, recognize, type + fieldcount? No, have to drive by the type and try to read!...only way.
         *  
         *  ----------------------------------------------
         *  The underlying reader must drive the setter. Columns are unpredictable. Properties are fixed.
         *  The type can know how each property must be set...e.g. SetIntProperty. This avoids the boxing penalty.
         *  Generate a builder that has a getter that knows the underlying type, gets it, then converts it explicitly. Needs a pipeline.
         *  There is no way around a dictionary to match properties.
         */ 
    }
}

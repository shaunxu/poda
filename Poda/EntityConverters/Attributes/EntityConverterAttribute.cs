using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.EntityConverters.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EntityConverterAttribute : Attribute
    {
        public Type Converter { get; set; }

        public EntityConverterAttribute(Type converter)
        {
            Converter = converter;
        }
    }
}

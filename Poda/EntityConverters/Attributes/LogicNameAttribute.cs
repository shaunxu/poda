using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.EntityConverters.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LogicNameAttribute : Attribute
    {
        public string Name { get; set; }

        public LogicNameAttribute()
            : this(string.Empty)
        {
        }

        public LogicNameAttribute(string name)
        {
            Name = name;
        }
    }
}

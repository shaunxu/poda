using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public class ReferenceElement : ConfigurationElement, IReferenceConfiguration
    {
        [ConfigurationProperty("table", IsKey = true, IsRequired = true)]
        public string Table
        {
            get
            {
                return (string)this["table"];
            }
            set
            {
                this["table"] = value;
            }
        }

        [ConfigurationProperty("dependency", DefaultValue = "")]
        public string Dependency
        {
            get
            {
                return (string)this["dependency"];
            }
            set
            {
                this["dependency"] = value;
            }
        }
    }
}

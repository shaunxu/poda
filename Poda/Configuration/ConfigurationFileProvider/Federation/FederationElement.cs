using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public class FederationElement : ConfigurationElement, IFederationConfiguration
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

        [ConfigurationProperty("column", IsRequired = true)]
        public string Column
        {
            get
            {
                return (string)this["column"];
            }
            set
            {
                this["column"] = value;
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

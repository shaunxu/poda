using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public class PolicyElement : ConfigurationElement, IPolicyConfiguration
    {
        [ConfigurationProperty("version", IsKey = true, IsRequired = true)]
        public string Version
        {
            get
            {
                return (string)this["version"];
            }
            set
            {
                this["version"] = value;
            }
        }

        [ConfigurationProperty("nodes", IsRequired = true)]
        public string Nodes
        {
            get
            {
                return (string)this["nodes"];
            }
            set
            {
                this["nodes"] = value;
            }
        }
    }
}

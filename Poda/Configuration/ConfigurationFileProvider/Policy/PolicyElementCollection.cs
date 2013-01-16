using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public class PolicyElementCollection : GenericElementCollection<PolicyElement>
    {
        public PolicyElementCollection()
            : base(elem => elem.Version)
        {
        }
    }
}

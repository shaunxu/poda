using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public class VersionProviderElementCollection : GenericElementCollection<VersionProviderElement>
    {
        public VersionProviderElementCollection()
            : base(elem => elem.Name)
        {
        }
    }
}

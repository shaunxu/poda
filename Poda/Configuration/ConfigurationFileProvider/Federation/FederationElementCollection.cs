using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public class FederationElementCollection : GenericElementCollection<FederationElement>
    {
        public FederationElementCollection()
            : base(elem => elem.Table)
        {
        }
    }
}

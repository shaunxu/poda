using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public class DispatcherElementCollection : GenericElementCollection<DispatcherElement>
    {
        public DispatcherElementCollection()
            : base(elem => elem.Name)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public class ReferenceElementCollection:GenericElementCollection<ReferenceElement>
    {
        public ReferenceElementCollection()
            : base(elem => elem.Table)
        {
        }
    }
}

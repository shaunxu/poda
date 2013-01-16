using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public class DatabaseElementCollection : GenericElementCollection<DatabaseElement>
    {
        public DatabaseElementCollection()
            : base(elem => elem.Name)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.Configuration
{
    public interface IConfigurationProvider
    {
        IPodaConfiguration Create();
    }
}

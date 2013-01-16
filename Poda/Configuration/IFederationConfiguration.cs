using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.Configuration
{
    public interface IFederationConfiguration
    {
        string Table { get; set; }
        string Column { get; set; }
        string Dependency { get; set; }
    }
}

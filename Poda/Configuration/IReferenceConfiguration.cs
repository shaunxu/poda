using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.Configuration
{
    public interface IReferenceConfiguration
    {
        string Table { get; set; }
        string Dependency { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.Configuration
{
    public interface IPolicyConfiguration
    {
        string Version { get; set; }
        string Nodes { get; set; }
    }
}

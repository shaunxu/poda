using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.Configuration
{
    public interface IPodaConfiguration
    {
        string Dispatcher { get; set; }
        int Retries { get; set; }
        string VersionProvider { get; set; }
        string Version { get; set; }

        IEnumerable<IDispatcherConfiguration> Dispatchers { get; set; }
        IEnumerable<IVersionProviderConfiguration> VersionProviders { get; set; }
        IEnumerable<IDatabaseConfiguration> Databases { get; set; }
        IEnumerable<IPolicyConfiguration> Policies { get; set; }
        IEnumerable<IFederationConfiguration> Federations { get; set; }
        IEnumerable<IReferenceConfiguration> References { get; set; }
    }
}

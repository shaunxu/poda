using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public class PodaConfigurationSection : ConfigurationSection, IPodaConfiguration
    {
        [ConfigurationProperty("dispatcher", IsRequired = true)]
        public string Dispatcher
        {
            get
            {
                return (string)this["dispatcher"];
            }
            set
            {
                this["dispatcher"] = value;
            }
        }

        [ConfigurationProperty("retries", IsRequired = true, DefaultValue = 3)]
        public int Retries
        {
            get
            {
                return (int)this["retries"];
            }
            set
            {
                this["retries"] = value;
            }
        }

        [ConfigurationProperty("versionProvider", IsRequired = true)]
        public string VersionProvider
        {
            get
            {
                return (string)this["versionProvider"];
            }
            set
            {
                this["versionProvider"] = value;
            }
        }

        [ConfigurationProperty("version", IsRequired = true)]
        public string Version
        {
            get
            {
                return (string)this["version"];
            }
            set
            {
                this["version"] = value;
            }
        }

        [ConfigurationProperty("dispatchers", IsRequired = true)]
        [ConfigurationCollection(typeof(DispatcherElementCollection), AddItemName = "dispatcher")]
        public DispatcherElementCollection Dispatchers
        {
            get
            {
                return (DispatcherElementCollection)this["dispatchers"];
            }
        }

        [ConfigurationProperty("versionProviders", IsRequired = true)]
        [ConfigurationCollection(typeof(VersionProviderElementCollection), AddItemName = "versionProvider")]
        public VersionProviderElementCollection VersionProviders
        {
            get
            {
                return (VersionProviderElementCollection)this["versionProviders"];
            }
        }

        [ConfigurationProperty("databases", IsRequired = true)]
        [ConfigurationCollection(typeof(DatabaseElementCollection), AddItemName = "database")]
        public DatabaseElementCollection Databases
        {
            get
            {
                return (DatabaseElementCollection)this["databases"];
            }
        }

        [ConfigurationProperty("policies", IsRequired = true)]
        [ConfigurationCollection(typeof(PolicyElementCollection), AddItemName = "policy")]
        public PolicyElementCollection Policies
        {
            get
            {
                return (PolicyElementCollection)this["policies"];
            }
        }

        [ConfigurationProperty("federations", IsRequired = true)]
        [ConfigurationCollection(typeof(FederationElementCollection), AddItemName = "federation")]
        public FederationElementCollection Federations
        {
            get
            {
                return (FederationElementCollection)this["federations"];
            }
        }

        [ConfigurationProperty("references", IsRequired = true)]
        [ConfigurationCollection(typeof(ReferenceElementCollection), AddItemName = "reference")]
        public ReferenceElementCollection References
        {
            get
            {
                return (ReferenceElementCollection)this["references"];
            }
        }

        #region IPodaConfiguration Methods

        IEnumerable<IDispatcherConfiguration> IPodaConfiguration.Dispatchers
        {
            get 
            {
                return Dispatchers;
            }
            set
            {
                throw new NotSupportedException("Set method is not supported for this configuration provider.");
            }
        }

        IEnumerable<IVersionProviderConfiguration> IPodaConfiguration.VersionProviders
        {
            get
            {
                return VersionProviders;
            }
            set
            {
                throw new NotSupportedException("Set method is not supported for this configuration provider.");
            }
        }

        IEnumerable<IDatabaseConfiguration> IPodaConfiguration.Databases
        {
            get
            {
                return Databases;
            }
            set
            {
                throw new NotSupportedException("Set method is not supported for this configuration provider.");
            }
        }

        IEnumerable<IFederationConfiguration> IPodaConfiguration.Federations
        {
            get
            {
                return Federations;
            }
            set
            {
                throw new NotSupportedException("Set method is not supported for this configuration provider.");
            }
        }

        IEnumerable<IReferenceConfiguration> IPodaConfiguration.References
        {
            get
            {
                return References;
            }
            set
            {
                throw new NotSupportedException("Set method is not supported for this configuration provider.");
            }
        }

        IEnumerable<IPolicyConfiguration> IPodaConfiguration.Policies
        {
            get
            {
                return Policies;
            }
            set
            {
                throw new NotSupportedException("Set method is not supported for this configuration provider.");
            }
        }

        #endregion
    }
}

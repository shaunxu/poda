using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poda.Configuration;
using Poda.DataEngine;
using Poda.Shared;
using System.Data;
using Poda.Share;

namespace Poda
{
    public class Factory
    {
        #region Singleton

        private static Factory _instance;
        private static Factory Current
        {
            get
            {
                var alock = new object();
                if (_instance == null)
                {
                    lock (alock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Factory();
                        }
                    }
                }
                return _instance;
            }
        }

        private Factory()
        {
            _configuration = null;
            _dispachers = new Dictionary<string, IDbDispatcher<DataSharding>>();
            _versionProvider = null;

            _retries = 0;
            _connectionConstructor = null;
        }

        #endregion

        private IPodaConfiguration _configuration;
        private IDictionary<string, IDbDispatcher<DataSharding>> _dispachers;
        private IVersionProvider _versionProvider;

        private int _retries;
        private Func<string, IDbConnection> _connectionConstructor;

        public static string CurrentVersion
        {
            get
            {
                return Current._versionProvider.CurrentVersion;
            }
        }

        private static IDbDispatcher<DataSharding> CreateDispatcher(Type dispatcherType, IEnumerable<IDatabaseConfiguration> databases)
        {
            var dispatcher = Activator.CreateInstance(dispatcherType) as IDbDispatcher<DataSharding>;
            dispatcher.KeySelector = ((sharding) => sharding.Name);
            dispatcher.Comparer = new DataSharding();
            dispatcher.Nodes = databases
                .Select(db => new DataSharding(db.Name, db.ConnectionString))
                .ToList();
            return dispatcher;
        }

        public static Factory Config(IConfigurationProvider provider, Func<string, IDbConnection> connectionConstructor)
        {
            // set the conguration information
            var config = provider.Create();
            Current._configuration = config;
            // load the dispatcher type for intialization
            var dispatcherName = config.Dispatcher;
            var dispatcherType = Type
                .GetType(config.Dispatchers.Where(dsp => dsp.Name == dispatcherName).First().Type)
                .MakeGenericType(typeof(DataSharding));
            // load the policies and the related database connection strings
            foreach (var policy in config.Policies)
            {
                var dispatcherKey = policy.Version;
                var databases = policy.Nodes.Split(';')
                    .Select((node) =>
                        config.Databases
                        .Where(db => db.Name == node)
                        .FirstOrDefault())
                    .ToList();
                Current._dispachers.Add(dispatcherKey, CreateDispatcher(dispatcherType, databases));
            }
            // load the currently version provider
            var versionProviderName = config.VersionProvider;
            var versionProviderType = Type
                .GetType(config.VersionProviders.Where(vp => vp.Name == versionProviderName).First().Type);
            Current._versionProvider = Activator.CreateInstance(versionProviderType) as IVersionProvider;
            Current._versionProvider.CurrentVersion = config.Version;
            // set the other properties
            Current._retries = config.Retries;
            Current._connectionConstructor = connectionConstructor;
            // return the singleton instance
            return Current;
        }

        public static IPodaEngine Create()
        {
            return Current.CreateImpl();
        }

        private IPodaEngine CreateImpl()
        {
            if (_configuration == null ||
                _connectionConstructor == null ||
                _dispachers == null ||
                _retries <= 0)
                throw new InvalidOperationException("Mandatory inner fields are empty please invoke Config before getting the engine.");

            return new PodaEngine(_dispachers, _versionProvider, _retries, _connectionConstructor);
        }
    }
}

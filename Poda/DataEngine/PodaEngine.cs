using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poda.Shared;
using System.Configuration;
using Poda.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Poda.Share;

namespace Poda.DataEngine
{
    internal class PodaEngine : IPodaEngine
    {
        private IDictionary<string, IDbDispatcher<DataSharding>> _dispachers;
        private IVersionProvider _versionProvider;

        private int _retries;
        private Func<string, IDbConnection> _connectionConstructor;

        // in fact it is no need to use concurrent collection
        // since when using fan-out operation the dispatcher will return the physical databases
        // rather than the virtual nodes so it's impossible that more than one threads
        // working on a same database but for future features and more safty
        // we utilize the concurrent dictionary here
        private ConcurrentDictionary<string, DataConnection> _connections;

        #region Constructors

        public PodaEngine(IDictionary<string, IDbDispatcher<DataSharding>> dispachers, IVersionProvider versionProvider, int retries, Func<string, IDbConnection> connectionConstructor)
        {
            _dispachers = dispachers;
            _versionProvider = versionProvider;

            _retries = retries;
            _connectionConstructor = connectionConstructor;

            _connections = new ConcurrentDictionary<string, DataConnection>();
        }

        #endregion

        public ICommandAfterConstruct Execute()
        {
            // create a command from the first connection in dispacher
            // when executing we will clone this command for the target database
            // so this command just for saving the command properties
            // and will NOT be executed directly
            var connectionString = _dispachers.First().Value.Nodes.First().ConnectionString;
            var connection = _connectionConstructor.Invoke(connectionString);
            var command = new Command(connection.CreateCommand(), this);
            return command;
        }

        #region Transaction Methods

        public void Commit()
        {
            foreach (var connection in _connections.Select(conn => conn.Value))
            {
                ExecuteWithRetries<DataConnection, int>(connection,
                    (conn) =>
                    {
                        if (conn.Transaction != null && conn.Transaction.Connection != null)
                        {
                            conn.Transaction.Commit();
                        }
                        return 0;
                    },
                    (conn) => { });
            }
        }

        public void Rollback()
        {
            foreach (var connection in _connections.Select(conn => conn.Value))
            {
                ExecuteWithRetries<DataConnection, int>(connection,
                    (conn) =>
                    {
                        if (conn.Transaction != null && conn.Transaction.Connection != null)
                        {
                            conn.Transaction.Rollback();
                        }
                        return 0;
                    },
                    (conn) => { });
            }
        }

        public void Dispose()
        {
            Rollback();

            foreach (var connection in _connections.Select(conn => conn.Value))
            {
                ExecuteWithRetries<DataConnection, int>(connection,
                    (conn) =>
                    {
                        if (conn.Connection != null)
                        {
                            conn.Connection.Close();
                        }
                        return 0;
                    },
                    (conn) => { });
            }
        }

        #endregion

        private IDbCommand CommandClone(IDbCommand command, IDbConnection connection, IDbTransaction transaction)
        {
            // create the command from the connection
            var result = connection.CreateCommand();
            // set the transaction for the new command
            result.Transaction = transaction;
            // clone the properties from the original command
            result.CommandText = command.CommandText;
            result.CommandType = command.CommandType;
            // copy the parameters into the new command
            foreach (var parameter in command.Parameters)
            {
                var dbDataParameter = parameter as IDbDataParameter;
                if (dbDataParameter == null)
                {
                    throw new NotSupportedException("The parameter must implement the IDbDataParameter interface in order to clone.");
                }
                else
                {
                    var targetParameter = result.CreateParameter();
                    targetParameter.ParameterName = dbDataParameter.ParameterName;
                    targetParameter.Value = dbDataParameter.Value;
                    result.Parameters.Add(targetParameter);
                }
            }
            return result;
        }

        #region Execute with Retries

        internal TResult Execute<TResult>(DataFederation federation, IDbCommand command, Func<IDbCommand, TResult> func, Func<IEnumerable<TResult>, TResult> merge)
        {
            var results = ExecuteCommand<TResult>(federation, command, func);
            return merge.Invoke(results);
        }

        private IEnumerable<DataSharding> DispatchDataShardings(DataFederation federation)
        {
            var version = _versionProvider.GetVersion(federation.Key);
            switch(federation.Type)
            {
                case DataFederation.FederationType.FanOut:
                    return _dispachers[version].Nodes;
                case DataFederation.FederationType.Reference:
                    return new DataSharding[] { _dispachers[version].RandomSelect() };
                default:
                    return new DataSharding[] { _dispachers[version].Dispatch(federation.Key) };
            }
        }

        private IEnumerable<TResult> ExecuteCommand<TResult>(DataFederation federation, IDbCommand command, Func<IDbCommand, TResult> func)
        {
            var shardings = DispatchDataShardings(federation);
            var options = new ParallelOptions();
            var results = new ConcurrentBag<TResult>();
            Parallel.ForEach(shardings, options, (sharding) =>
            {
                // open the connection without any compensation
                var connection = _connectionConstructor.Invoke(sharding.ConnectionString);
                var dataConnection = _connections.GetOrAdd(sharding.Name, new DataConnection(sharding, connection));
                ExecuteWithRetries<DataConnection, int>(dataConnection,
                    (conn) =>
                    {
                        if (conn.Connection.State == ConnectionState.Closed)
                        {
                            conn.Connection.Open();
                        }
                        if (conn.Transaction == null)
                        {
                            conn.Transaction = conn.Connection.BeginTransaction();
                        }
                        return 0;
                    },
                    (ce) => { });
                // execute the command with the compensation re-openning
                var cloned_command = CommandClone(command, dataConnection.Connection, dataConnection.Transaction);
                var result = ExecuteWithRetries<IDbCommand, TResult>(cloned_command, func, (cmd) =>
                {
                    if (cmd.Connection.State == ConnectionState.Broken || cmd.Connection.State == ConnectionState.Closed)
                    {
                        cmd.Connection.Open();
                        cmd.Transaction = cmd.Connection.BeginTransaction();
                    }
                });
                // save the result into the concurrent bag
                results.Add(result);
            });
            return results;
        }

        private TResult ExecuteWithRetries<TCommand, TResult>(TCommand instance, Func<TCommand, TResult> action, Action<TCommand> compensation)
        {
            int retry = 0;
            bool success = false;
            var exceptions = new List<Exception>();
            TResult result = default(TResult);
            // try to execute the action within the retry count
            while (!success && retry < _retries)
            {
                try
                {
                    result = action.Invoke(instance);
                    success = true;
                }
                catch (SqlException sqlex)
                {
                    // save the exception
                    exceptions.Add(sqlex);
                    // tried to execute the compensation 
                    // this will be wrapped by try-catch and will not thorw any exceptions
                    try
                    {
                        compensation.Invoke(instance);
                    }
                    catch { }
                }
                retry++;
            }
            // throw exception if failed out of the retry count
            if (!success)
            {
                throw new ExecuteWithRetriesException("Exception(s) occurred when executing the command out of the retry count.", exceptions);
            }
            return result;
        }

        #endregion
    }
}

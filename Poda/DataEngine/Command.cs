using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poda.Shared;
using System.Data;
using System.Data.SqlClient;

namespace Poda.DataEngine
{
    internal class Command : ICommandAfterConstruct, ICommandAfterForSQLOrSPROC, ICommandAfterWith, ICommandAfterFederation
    {
        private IDbCommand _command;
        private PodaEngine _explorer;
        private DataFederation _federation;

        internal Command(IDbCommand command, PodaEngine explorer)
        {
            _command = command;
            _explorer = explorer;
            _federation = new DataFederation(false);
        }

        #region ForPlainSQL and ForStoredProcedure

        public ICommandAfterForSQLOrSPROC ForPlainSQL(string text)
        {
            _command.CommandText = text;
            _command.CommandType = CommandType.Text;
            return this;
        }

        public ICommandAfterForSQLOrSPROC ForStoredProcedure(string text)
        {
            _command.CommandText = text;
            _command.CommandType = CommandType.StoredProcedure;
            return this;
        }

        #endregion

        #region With

        public ICommandAfterWith With(IDbDataParameter paramater)
        {
            _command.Parameters.Add(paramater);
            return this;
        }

        public ICommandAfterWith With(string parameterName, object parameterValue)
        {
            return With(new SqlParameter(parameterName, parameterValue));
        }

        #endregion

        #region FederationOn

        public ICommandAfterFederation ReferenceOn(string tableName)
        {
            _federation = new DataFederation(true);
            return this;
        }

        public ICommandAfterFederation FederationOn(string tableName, string federationColumn, string federationKey)
        {
            _federation = new DataFederation(tableName, federationColumn, federationKey, false);
            return this;
        }

        public ICommandAfterFederation FederationOnAll()
        {
            _federation = new DataFederation(false);
            return this;
        }

        #endregion

        #region As

        private T As<T>(Func<IDbCommand, T> action, Func<IEnumerable<T>, T> merge)
        {
            return _explorer.Execute<T>(_federation, _command, action, merge);
        }

        public void AsNothing()
        {
            As<int>(
                (cmd) =>
                {
                    return cmd.ExecuteNonQuery();
                },
                (results) =>
                {
                    return results.Sum();
                });
        }

        public decimal AsScopeIdentity()
        {
            return As<Decimal>(
                (cmd) =>
                {
                    cmd.CommandText += "\nSELECT SCOPE_IDENTITY()";
                    return As<decimal>();
                },
                (results) =>
                {
                    return results.First();
                });
        }

        public DataSet AsDataSet(IDbDataAdapter adapter)
        {
            return As<DataSet>(
                (cmd) =>
                {
                    var ds = new DataSet();
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds);
                    return ds;
                },
                (results) =>
                {
                    var result = new DataSet();
                    // clone the first table into the dataset so the schema will be copied
                    if (results.Count() > 0 && results.First().Tables.Count > 0)
                    {
                        result.Tables.Add(results.First().Tables[0].Clone());
                    }
                    // copy all rows from all tables
                    // still need to copy the first one as we only copied the structure before
                    foreach (var ds in results)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            foreach (var row in ds.Tables[0].AsEnumerable())
                            {
                                result.Tables[0].ImportRow(row);
                            }
                        }
                    }
                    return result;
                });
        }

        public DataSet AsDataSet()
        {
            return AsDataSet(new SqlDataAdapter());
        }

        public T As<T>()
        {
            return As<T>(
                (cmd) =>
                {
                    return (T)cmd.ExecuteScalar();
                },
                (results) =>
                {
                    return results.First();
                });
        }

        public IEnumerable<T> AsEntities<T>(IEntityConverter converter) where T : new()
        {
            return As<IEnumerable<T>>(
                (cmd) =>
                {
                    var result = new List<T>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(converter.Convert<T>(reader));
                        }
                    }
                    return result;
                },
                (results) =>
                {
                    var result = new List<T>();
                    foreach (var entities in results)
                    {
                        result.AddRange(entities);
                    }
                    return result;
                });
        }

        public IEnumerable<T> AsEntities<T>(IEntityConverter<T> converter) where T : new()
        {
            return As<IEnumerable<T>>(
                (cmd) =>
                {
                    var result = new List<T>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(converter.Convert(reader));
                        }
                    }
                    return result;
                },
                (results) =>
                {
                    var result = new List<T>();
                    foreach (var entities in results)
                    {
                        result.AddRange(entities);
                    }
                    return result;
                });
        }

        public IEnumerable<T> AsEntities<T>(Func<IDataReader, T> converter)
        {
            return As<IEnumerable<T>>(
                (cmd) =>
                {
                    var result = new List<T>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(converter.Invoke(reader));
                        }
                    }
                    return result;
                },
                (results) =>
                {
                    var result = new List<T>();
                    foreach (var entities in results)
                    {
                        result.AddRange(entities);
                    }
                    return result;
                });
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Poda.Shared
{
    public interface ICommandAfterConstruct
    {
        ICommandAfterForSQLOrSPROC ForPlainSQL(string text);
        ICommandAfterForSQLOrSPROC ForStoredProcedure(string text);
    }

    public interface ICommandAfterForSQLOrSPROC : ICommandAfterWith
    {
    }

    public interface ICommandAfterWith
    {
        ICommandAfterWith With(IDbDataParameter paramater);
        ICommandAfterWith With(string parameterName, object parameterValue);

        ICommandAfterFederation ReferenceOn(string tableName);
        ICommandAfterFederation FederationOn(string tableName, string federationColumn, string federationKey);
        ICommandAfterFederation FederationOnAll();

    }

    public interface ICommandAfterFederation
    {
        void AsNothing();
        decimal AsScopeIdentity();
        DataSet AsDataSet(IDbDataAdapter adapter);
        DataSet AsDataSet();
        T As<T>();
        IEnumerable<T> AsEntities<T>(IEntityConverter converter) where T : new();
        IEnumerable<T> AsEntities<T>(IEntityConverter<T> converter) where T : new();
        IEnumerable<T> AsEntities<T>(Func<IDataReader, T> converter);
    }
}

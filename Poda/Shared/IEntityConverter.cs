using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Poda.Shared
{
    public interface IEntityConverter
    {
        TEntity Convert<TEntity>(IDataRecord reader) where TEntity : new();
    }

    public interface IEntityConverter<TEntity> where TEntity : new()
    {
        TEntity Convert(IDataRecord reader);
    }
}

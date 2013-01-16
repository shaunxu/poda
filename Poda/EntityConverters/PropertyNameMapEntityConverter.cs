using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poda.Shared;
using System.Data;

namespace Poda.EntityConverters
{
    public class PropertyNameMapEntityConverter : IEntityConverter
    {
        public TEntity Convert<TEntity>(IDataRecord reader) where TEntity : new()
        {
            var entity = new TEntity();
            var properties = typeof(TEntity).GetProperties();
            for (var i = 0; i < reader.FieldCount - 1; i++)
            {
                var fieldName = reader.GetName(i);
                var fieldType = reader.GetFieldType(i);
                var fieldValue = reader.GetValue(i);
                if (fieldValue != null && fieldValue != DBNull.Value)
                {
                    var property = properties
                        .Where(p => string.Compare(p.Name, fieldName, true) == 0 && p.PropertyType == fieldType)
                        .FirstOrDefault();
                    if (property != null)
                    {
                        property.SetValue(entity, fieldValue, null);
                    }
                }
            }
            return entity;
        }
    }
}

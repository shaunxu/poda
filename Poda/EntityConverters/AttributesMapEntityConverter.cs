using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poda.Shared;
using System.Data;
using Poda.EntityConverters.Attributes;

namespace Poda.EntityConverters
{
    public class AttributesMapEntityConverter : IEntityConverter
    {
        public TEntity Convert<TEntity>(IDataRecord reader) where TEntity : new()
        {
            var entity = new TEntity();
            foreach (var property in typeof(TEntity).GetProperties())
            {
                var logicNameAttrib = property.GetCustomAttributes(typeof(LogicNameAttribute), true)
                    .Select(a => a as LogicNameAttribute)
                    .FirstOrDefault();
                if (logicNameAttrib != null)
                {
                    var fieldName = logicNameAttrib.Name;
                    var fieldValue = reader[fieldName];
                    property.SetValue(entity, fieldValue, null);
                }
            }
            return entity;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poda.Shared;
using System.Data;
using Poda.EntityConverters.Attributes;

namespace Poda.EntityConverters
{
    public class SpecifiedEntityConverter : IEntityConverter
    {
        public TEntity Convert<TEntity>(IDataRecord reader) where TEntity : new()
        {
            var entity = new TEntity();
            var attrib = typeof(TEntity).GetCustomAttributes(typeof(EntityConverterAttribute), true)
                .Select(a => a as EntityConverterAttribute)
                .FirstOrDefault();
            if (attrib == null)
            {
                throw new ArgumentException("Cannot find the related EntityConverterAttribute if specified SpecifiedEntityConverter.");
            }
            else
            {
                var converter = Activator.CreateInstance(attrib.Converter) as IEntityConverter<TEntity>;
                if (converter == null)
                {
                    throw new ArgumentException("The converter specified in EntityConverterAttribute must be inherited from IEntityConverter<TEntity>.");
                }
                else
                {
                    entity = converter.Convert(reader);
                }
            }

            return entity;
        }
    }
}

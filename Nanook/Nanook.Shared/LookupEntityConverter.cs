using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poda.Shared;
using System.Data;

namespace Nanook.Shared
{
    public class LookupEntityConverter : IEntityConverter<KeyValuePair<Guid, string>>
    {
        private string _keyColumn;
        private string _valueColumn;

        public LookupEntityConverter(string keyColumn, string valueColumn)
        {
            _keyColumn = keyColumn;
            _valueColumn = valueColumn;
        }


        public KeyValuePair<Guid, string> Convert(IDataRecord reader)
        {
            return new KeyValuePair<Guid, string>((Guid)reader[_keyColumn], (string)reader[_valueColumn]);
        }
    }
}

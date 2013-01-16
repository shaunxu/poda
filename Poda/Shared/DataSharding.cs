using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.Share
{
    internal class DataSharding : IEqualityComparer<DataSharding>
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        public DataSharding()
            : this(string.Empty, string.Empty)
        {
        }

        public DataSharding(string name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;
        }

        public bool Equals(DataSharding x, DataSharding y)
        {
            return string.Compare(x.Name, y.Name, true) == 0;
        }

        public int GetHashCode(DataSharding obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}

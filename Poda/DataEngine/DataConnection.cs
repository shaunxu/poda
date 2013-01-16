using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poda.Share;
using System.Data;

namespace Poda.DataEngine
{
    internal class DataConnection
    {
        public DataSharding Sharding { get; set; }
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }

        public DataConnection(DataSharding sharding, IDbConnection connection)
        {
            Sharding = sharding;
            Connection = connection;
            Transaction = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.DataEngine
{
    internal class DataFederation
    {
        public enum FederationType
        {
            Fedration,
            FanOut,
            Reference
        }

        public string Table { get; set; }
        public string Column { get; set; }
        public string Key { get; set; }
        public bool IsReference { get; set; }

        public FederationType Type
        {
            get
            {
                if (IsReference)
                {
                    return FederationType.Reference;
                }
                else if (string.IsNullOrWhiteSpace(Table) || string.IsNullOrWhiteSpace(Column) || string.IsNullOrWhiteSpace(Key))
                {
                    return FederationType.FanOut;
                }
                else
                {
                    return FederationType.Fedration;
                }
            }
        }

        public DataFederation(bool reference)
            : this(string.Empty, string.Empty, string.Empty, reference)
        {
        }

        public DataFederation(string table, string column, string key, bool reference)
        {
            Table = table;
            Column = column;
            Key = key;
            IsReference = reference;
        }
    }
}

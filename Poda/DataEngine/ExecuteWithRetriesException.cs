using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.DataEngine
{
    public sealed class ExecuteWithRetriesException : Exception
    {
        public IEnumerable<Exception> InnerExceptions { get; private set; }

        public ExecuteWithRetriesException(string message, IEnumerable<Exception> exceptions)
            : base(message)
        {
            InnerExceptions = exceptions;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.Shared
{
    public interface IPodaEngine : IDisposable
    {
        ICommandAfterConstruct Execute();

        void Commit();

        void Rollback();
    }
}

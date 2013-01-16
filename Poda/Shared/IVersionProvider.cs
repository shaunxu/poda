using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.Shared
{
    public interface IVersionProvider
    {
        string CurrentVersion { get; set; }

        string GetVersion(string key);

        void SetVersion(string key, string version);
    }
}

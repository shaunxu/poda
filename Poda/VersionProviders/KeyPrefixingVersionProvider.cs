using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poda.Shared;

namespace Poda.VersionProviders
{
    public class KeyPrefixingVersionProvider : IVersionProvider
    {
        private const char CST_DEFAULT_DELIMITER = '|';
        private char _delimiter;
        private string _currentVersion;

        public string CurrentVersion
        {
            get
            {
                return _currentVersion;
            }
            set
            {
                _currentVersion = value;
            }
        }

        public KeyPrefixingVersionProvider()
            : this(CST_DEFAULT_DELIMITER)
        {
        }

        public KeyPrefixingVersionProvider(char delimiter)
        {
            _delimiter = delimiter;
        }

        public string GetVersion(string key)
        {
            var sources = key.Split(_delimiter);
            if (sources.Length > 1)
            {
                return sources[0];
            }
            else
            {
                return _currentVersion;
            }
        }

        public void SetVersion(string key, string version)
        {
            throw new NotSupportedException("The KeyPrefixingVersionProvider doesn't support SetVersion. The version information will be stored with in the key.");
        }
    }

    #region Extension Method for Key Generation

    public static class KeyPrefixingVersionExtension
    {
        public static string GetKeyWithVersion(this Guid key, string version)
        {
            return string.Format("{0}|{1}", version, key.ToString());
        }
    }

    #endregion

}

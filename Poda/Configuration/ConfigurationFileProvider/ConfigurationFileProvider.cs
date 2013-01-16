using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public class ConfigurationFileProvider : IConfigurationProvider
    {
        private const string CST_DEFAULT_SECTION_NAME = "poda";
        private string _sectionName;

        public ConfigurationFileProvider()
            : this(CST_DEFAULT_SECTION_NAME)
        {
        }

        public ConfigurationFileProvider(string sectionName)
        {
            _sectionName = sectionName;
        }

        public IPodaConfiguration Create()
        {
            return ConfigurationManager.GetSection(_sectionName) as PodaConfigurationSection;
        }
    }
}

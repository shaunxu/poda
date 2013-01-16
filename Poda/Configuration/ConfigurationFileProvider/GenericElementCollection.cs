using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Poda.Configuration.ConfigurationFileProvider
{
    public abstract class GenericElementCollection<T> : ConfigurationElementCollection, IEnumerable<T> where T : ConfigurationElement, new()
    {
        private Func<T, object> _keySelector;

        protected GenericElementCollection(Func<T, object> keySelector)
        {
            _keySelector = keySelector;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return _keySelector.Invoke((T)element);
        }

        public new T this[string name]
        {
            get
            {
                return (T)BaseGet(name);
            }
        }

        public new IEnumerator<T> GetEnumerator()
        {
            foreach (var key in BaseGetAllKeys())
            {
                yield return (T)BaseGet(key);
            }
        }
    }
}

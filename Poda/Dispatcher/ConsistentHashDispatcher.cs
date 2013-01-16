using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Poda.Shared;

namespace Poda.Dispatcher
{
    public class ConsistentHashDispatcher<TNode> : IDbDispatcher<TNode>
    {
        private const long CST_ONENODE_KEY = 0;

        private SortedList<long, TNode> _virtualNodes;
        private Func<TNode, string> _keySelector;
        private IEqualityComparer<TNode> _comparer;
        private int _virtualCopies;

        public IEnumerable<TNode> Nodes
        {
            get
            {
                if (_virtualNodes.Count == 1)
                {
                    return new TNode[] { _virtualNodes[CST_ONENODE_KEY] };
                }
                else
                {
                    return _virtualNodes
                        .Select(n => n.Value)
                        .Distinct(_comparer)
                        .ToList();
                }
            }
            set
            {
                CreateKetamaNodes(value);
            }
        }

        public Func<TNode, string> KeySelector
        {
            get
            {
                return _keySelector;
            }
            set
            {
                _keySelector = value;
            }
        }

        public IEqualityComparer<TNode> Comparer
        {
            get
            {
                return _comparer;
            }
            set
            {
                _comparer = value;
            }
        }

        public ConsistentHashDispatcher()
        {
            _virtualCopies = 4;
            _virtualNodes = new SortedList<long, TNode>();
            _comparer = null;
        }

        private void CreateKetamaNodes(IEnumerable<TNode> nodes)
        {
            if (nodes.Count() == 1)
            {
                // speed up for only one node
                _virtualNodes.Add(CST_ONENODE_KEY, nodes.First());
            }
            else
            {
                // ketama hash
                foreach (var node in nodes)
                {
                    for (var i = 0; i < _virtualCopies / 4; i++)
                    {
                        var digest = ComputeMd5(_keySelector.Invoke(node) + i);
                        for (var h = 0; h < 4; h++)
                        {
                            var m = ComputeHash(digest, h);
                            _virtualNodes.Add(m, node);
                        }
                    }
                }
            }
        }

        public TNode Dispatch(string dispatchKey)
        {
            if (string.IsNullOrWhiteSpace(dispatchKey))
                throw new ArgumentNullException("dispatchKey", "The dispatchKey cannot be null, empty or white spaces.");

            if (_virtualNodes.Count == 1)
            {
                // speed up for only one node
                return _virtualNodes[CST_ONENODE_KEY];
            }
            else
            {
                // consistent hash dispatch
                var digest = ComputeMd5(dispatchKey);
                var hash = ComputeHash(digest, 0);
                return Dispatch(hash);
            }
        }

        private TNode Dispatch(long hash)
        {
            TNode node = default(TNode);
            if (_virtualNodes.ContainsKey(hash))
            {
                node = _virtualNodes[hash];
            }
            else
            {
                node = _virtualNodes
                    .Where(n => n.Key > hash)
                    .Select(n => n.Value)
                    .FirstOrDefault();
                if (node == null)
                {
                    node = _virtualNodes.First().Value;
                }
            }
            return node;
        }

        public TNode RandomSelect()
        {
            if (_virtualNodes.Count == 1)
            {
                // speed up for only one database
                return _virtualNodes[CST_ONENODE_KEY];
            }
            else
            {
                // random select a database
                var rnd = new Random();
                var buffer = new byte[sizeof(long)];
                rnd.NextBytes(buffer);
                var hash = BitConverter.ToInt64(buffer, 0);
                return Dispatch(hash);
            }
        }

        #region Utilities

        private byte[] ComputeMd5(string k)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] keyBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(k));
            md5.Clear();
            return keyBytes;
        }

        private long ComputeHash(byte[] digest, int nTime)
        {
            long rv = ((long)(digest[3 + nTime * 4] & 0xFF) << 24)
                    | ((long)(digest[2 + nTime * 4] & 0xFF) << 16)
                    | ((long)(digest[1 + nTime * 4] & 0xFF) << 8)
                    | ((long)digest[0 + nTime * 4] & 0xFF);
            return rv & 0xFFFFFFFFL; /* Truncate to 32-bits */
        }

        #endregion
    }
}

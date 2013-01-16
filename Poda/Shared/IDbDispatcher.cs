using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poda.Shared
{
    public interface IDbDispatcher<TNode>
    {
        IEnumerable<TNode> Nodes { get; set; }

        Func<TNode, string> KeySelector { get; set; }

        IEqualityComparer<TNode> Comparer { get; set; }

        TNode Dispatch(string dispatchKey);

        TNode RandomSelect();
    }
}

using System;
using UnityEngine;

namespace Logical99.Runtime
{
    /// <summary>
    /// Structural node that marks the start of a graph and immediately forwards execution.
    /// </summary>
    [Serializable]
    public sealed class EntryNode : ANode
    {
        public override void OnNodeEnter(NodeExecutionContext context)
        {
            context.TraverseEdge(0);
        }
    }
}

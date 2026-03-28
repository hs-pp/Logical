using System;

namespace Logical99.Runtime
{
    /// <summary>
    /// Provides runtime-only access to graph execution services and mutable per-node state.
    /// Node definitions should use this instead of mutating serialized node data.
    /// </summary>
    public sealed class NodeExecutionContext
    {
        private readonly NodeRuntimeState m_runtimeState;
        private readonly Action<ANode, OutportEdge> m_requestTransition;
        
        public ANode Node { get; }
        public NodeRuntimeState RuntimeState => m_runtimeState;
        public AGraphProperties GraphProperties { get; }
        
        public NodeExecutionContext(
            ANode node,
            AGraphProperties graphProperties,
            NodeRuntimeState runtimeState,
            Action<ANode, OutportEdge> requestTransition)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            GraphProperties = graphProperties;
            m_runtimeState = runtimeState;
            m_requestTransition = requestTransition ?? throw new ArgumentNullException(nameof(requestTransition));
        }

        public TState GetState<TState>() where TState : NodeRuntimeState
        {
            return RuntimeState as TState;
        }

        /// <summary>
        /// Call this method from any of a node's methods to traverse a particular edge and enter another node.
        /// </summary>
        /// <param name="index">The index of the edge (aka outport) we want to traverse.</param>
        public void TraverseEdge(int index)
        {
            m_requestTransition.Invoke(Node, Node.GetOutportEdge(index));
        }

        public void StopGraph()
        {
            m_requestTransition.Invoke(Node, null);
        }
    }
}

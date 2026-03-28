using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logical99.Runtime
{
    /// <summary>
    /// The class that actually runs a graph.
    /// This class has three essential methods that control the lifetime of a graph:
    /// 1. StartGraph(): Starts the graph, beginning at the graph's EntryNode.
    /// 2. UpdateGraph(): Calls OnUpdateNode() on the currently active node in the graph. This effectively does nothing if none of your nodes require OnUpdateNode().
    /// 3. StopGraph(): Stops the graph and exits out of the current node properly.
    /// 
    /// This class does not need to be attached to a monobehaviour, but it'll need someone to call it's UpdateGraph()
    /// method assuming the graph that is running needs updates called on its nodes.
    /// 
    /// NodeGraphController is a basic monobehaviour that uses a GraphRunner to execute graphs at runtime.
    /// </summary>
    public class GraphRunner
    {
        private NodeCollection m_nodeCollection = null;
        private readonly Dictionary<string, NodeRuntimeState> m_nodeRuntimeStates = new Dictionary<string, NodeRuntimeState>();
        private PendingTransition m_pendingTransition;
        private bool m_isProcessingNodeCallback = false;
        public AGraphProperties GraphProperties { get; private set; }
        public bool IsRunning => m_currentNode != null;
        public ANode CurrentNode => m_currentNode;

        [NonSerialized]
        public Action OnGraphStart = null;
        [NonSerialized]
        public Action OnGraphStop = null;
        [NonSerialized]
        public Action<ANode> OnNodeChange = null;
        private ANode m_currentNode = null;

        private struct PendingTransition
        {
            public ANode SourceNode;
            public OutportEdge Edge;
            public bool Exists;
        }

        public GraphRunner(ANodeGraph nodeGraph, AGraphProperties graphProperties)
        {
            m_nodeCollection = nodeGraph.NodeCollection;
            GraphProperties = graphProperties;

            if (m_nodeCollection == null)
            {
                Debug.LogError("No Graph attached to GraphRunner!");
            }
        }

        public GraphRunner(NodeCollection nodeCollection, AGraphProperties graphProperties)
        {
            m_nodeCollection = nodeCollection;
            GraphProperties = graphProperties;
        }

        public void StartGraph()
        {
            m_pendingTransition = default;
            m_nodeRuntimeStates.Clear();
            m_currentNode = null;

            if (m_nodeCollection == null)
            {
                Debug.LogError("GraphRunner could not start because no node collection was provided.");
                OnNodeChange?.Invoke(null);
                return;
            }

            m_currentNode = m_nodeCollection.GetEntryNode();
            if (m_currentNode == null)
            {
                Debug.LogError("GraphRunner could not start because no entry node was found.");
                OnNodeChange?.Invoke(null);
                return;
            }

            OnGraphStart?.Invoke();
            OnNodeChange?.Invoke(m_currentNode);
            ExecuteNodeCallback(m_currentNode, node => node.OnNodeEnter(CreateNodeExecutionContext(node)));
            CommitPendingTransition();
        }

        public void StopGraph()
        {
            if (m_currentNode == null)
            {
                return;
            }

            ExecuteNodeCallback(m_currentNode, node => node.OnNodeExit(CreateNodeExecutionContext(node)));
            m_currentNode = null;
            m_pendingTransition = default;
            OnGraphStop?.Invoke();
            OnNodeChange?.Invoke(null);
        }

        public void UpdateGraph()
        {
            if (m_currentNode == null)
            {
                return;
            }

            if (m_pendingTransition.Exists)
            {
                CommitPendingTransition();
            }
            else
            {
                ExecuteNodeCallback(m_currentNode, node => node.OnNodeUpdate(CreateNodeExecutionContext(node)));
            }
        }

        private void RequestTransition(ANode node, OutportEdge edge)
        {
            if (!m_isProcessingNodeCallback)
            {
                Debug.LogError("Transitions can only be requested during node execution.");
                return;
            }

            if (node != m_currentNode)
            {
                Debug.LogError("Ahhhh! Trying to traverse edge from non-current node.");
                return;
            }

            if (!m_currentNode.ContainsOutport(edge))
            {
                Debug.LogError("Ahhhh! Trying to traverse edge from non-current node.");
                return;
            }

            if (m_pendingTransition.Exists)
            {
                Debug.LogWarning("A transition was already requested during this node callback. Ignoring the later request.");
                return;
            }

            m_pendingTransition = new PendingTransition
            {
                SourceNode = node,
                Edge = edge,
                Exists = true
            };
        }

        private void CommitPendingTransition()
        {
            if (!m_pendingTransition.Exists || m_currentNode == null)
            {
                return;
            }

            PendingTransition transition = m_pendingTransition;
            m_pendingTransition = default;

            ExecuteNodeCallback(m_currentNode, node => node.OnNodeExit(CreateNodeExecutionContext(node)));

            if (transition.Edge == null)
            {
                m_currentNode = null;
                OnGraphStop?.Invoke();
                OnNodeChange?.Invoke(null);
                return;
            }

            ANode nextNode = m_nodeCollection.GetNodeById(transition.Edge.ConnectedNodeId);
            if (nextNode == null)
            {
                Debug.LogError($"Transition target node '{transition.Edge.ConnectedNodeId}' was not found.");
                m_currentNode = null;
                OnGraphStop?.Invoke();
                OnNodeChange?.Invoke(null);
                return;
            }

            m_currentNode = nextNode;
            OnNodeChange?.Invoke(m_currentNode);
            ExecuteNodeCallback(m_currentNode, node => node.OnNodeEnter(CreateNodeExecutionContext(node)));
            CommitPendingTransition();
        }

        private NodeExecutionContext CreateNodeExecutionContext(ANode node)
        {
            if (node == null)
            {
                return null;
            }

            // Get node state from dictionary
            if (!m_nodeRuntimeStates.TryGetValue(node.Id, out NodeRuntimeState state))
            {
                state = node.CreateRuntimeState() ?? new DefaultNodeRuntimeState();
                m_nodeRuntimeStates.Add(node.Id, state);
            }
            
            return new NodeExecutionContext(node, GraphProperties, state, RequestTransition);
        }

        private void ExecuteNodeCallback(ANode node, Action<ANode> callback)
        {
            if (node == null)
            {
                return;
            }

            m_isProcessingNodeCallback = true;

            try
            {
                callback.Invoke(node);
            }
            finally
            {
                m_isProcessingNodeCallback = false;
            }
        }
    }
}

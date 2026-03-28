using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logical.Runtime
{
    [Serializable]
    public class NodeCollection
    {
        [SerializeReference]
        private List<ANode> m_nodes = new List<ANode>();

        [NonSerialized]
        private Dictionary<string, ANode> m_nodeCache;

        [NonSerialized]
        private EntryNode m_entryNode;
        
        public ANode GetNodeById(string id)
        {
            if (m_nodeCache == null)
            {
                BuildNodeCache();
            }

            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            
            m_nodeCache.TryGetValue(id, out ANode node);
            return node;
        }
        
        public EntryNode GetEntryNode()
        {
            if (m_nodeCache == null)
            {
                BuildNodeCache();
            }

            return m_entryNode;
        }

        private void BuildNodeCache()
        {
            if (m_nodeCache != null)
            {
                return;
            }

            m_nodeCache = new Dictionary<string, ANode>(m_nodes.Count);
            m_entryNode = null;
            bool foundMultipleEntryNodes = false;

            foreach (ANode node in m_nodes)
            {
                if (node == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(node.Id) && !m_nodeCache.TryAdd(node.Id, node))
                {
                    Debug.LogError($"Duplicate node id '{node.Id}' found.");
                }

                EntryNode entryNode = node as EntryNode;
                if (entryNode == null)
                {
                    continue;
                }

                if (m_entryNode == null)
                {
                    m_entryNode = entryNode;
                    continue;
                }

                foundMultipleEntryNodes = true;
            }

            if (foundMultipleEntryNodes)
            {
                m_entryNode = null;
                Debug.LogError("Multiple EntryNode instances found in the same NodeCollection.");
            }

            if (m_entryNode == null && !foundMultipleEntryNodes)
            {
                Debug.LogError("No EntryNode found in NodeCollection.");
            }
        }
    }
}

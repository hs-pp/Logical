using Logical.Runtime;
using UnityEditor;

namespace Logical.Editor
{
    /// <summary>
    /// This class takes a NodeGraph ScriptableObject, creates a SerializedObject for it, and exposes easy to use
    /// functions to manipulate the data within the NodeGraph SerializedObject and ultimately saving it back to the ScriptableObject.
    /// This class is to only be used by the NodeGraph editor.
    /// </summary>
    internal class NodeGraphHandler
    {
        private ANodeGraph m_nodeGraph;
        private SerializedObject m_serializedGraph;
        
        public NodeGraphHandler(ANodeGraph nodeGraph)
        {
            m_nodeGraph = nodeGraph;
            m_serializedGraph = new SerializedObject(nodeGraph);
        }

        
        public void SaveNodeGraph()
        {
            m_serializedGraph.ApplyModifiedProperties();
        }
    }
}
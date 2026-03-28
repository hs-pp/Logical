using Logical.Runtime;
using UnityEngine.UIElements;

namespace Logical.Editor
{
    /// <summary>
    /// The core VisualElement representing the grid, the minimap, the nodes, and various other graph elements.
    /// This is also where we handle the various types of controls like mouse clicks and drags.
    /// </summary>
    public class GraphView : VisualElement
    {
        private NodeGraphHandler m_currNodeGraph;
        
        public void LoadNodeGraph(ANodeGraph nodeGraph)
        {
            if (m_currNodeGraph != null)
            {
                // UNLOAD PREVIOUS NODE GRAPH!!
            }
            
            m_currNodeGraph = new NodeGraphHandler(nodeGraph);
            // DO SOME LOADING OF DATA!!
        }
    }
}
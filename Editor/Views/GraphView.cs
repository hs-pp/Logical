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
            ClearGraphView();
            
            m_currNodeGraph = new NodeGraphHandler(nodeGraph);
            // DO SOME LOADING OF DATA!!
        }

        public void ClearGraphView()
        {
            if (m_currNodeGraph == null)
            {
                return;
            }
            
            
        }
    }
}
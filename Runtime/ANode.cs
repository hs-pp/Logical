using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logical99.Runtime
{
    [Serializable]
    public abstract class ANode
    {
        [SerializeField, HideInInspector]
        private string m_id; // = Guid.NewGuid().ToString(); This is generated from the editor side.
        public string Id => m_id;
        
        [SerializeField, HideInInspector]
        private List<OutportEdge> m_outports = new List<OutportEdge>(0);

        public virtual NodeRuntimeState CreateRuntimeState()
        {
            return new DefaultNodeRuntimeState();
        }

        public virtual void OnNodeEnter(NodeExecutionContext context) { }
        public virtual void OnNodeUpdate(NodeExecutionContext context) { }
        public virtual void OnNodeExit(NodeExecutionContext context) { }

        public bool ContainsOutport(OutportEdge outportEdge)
        {
            if (outportEdge == null)
            {
                return true;
            }

            return m_outports.Exists(x => x.Id == outportEdge.Id);
        }

        public OutportEdge GetOutportEdge(int index)
        {
            return m_outports[index];
        }
    }
}

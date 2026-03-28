using System;
using UnityEngine;

namespace Logical99.Runtime
{
    [Serializable]
    public sealed class SubGraphNode : ANode
    {
        [SerializeField]
        private NodeCollection m_subGraph = null;

        [SerializeField]
        private int m_onCompleteOutportIndex = 0;

        public NodeCollection SubGraph => m_subGraph;

        public override NodeRuntimeState CreateRuntimeState()
        {
            return new SubGraphNodeRuntimeState();
        }

        public override void OnNodeEnter(NodeExecutionContext context)
        {
            SubGraphNodeRuntimeState state = context.GetState<SubGraphNodeRuntimeState>();
            if (state == null || m_subGraph == null)
            {
                context.StopGraph();
                return;
            }

            state.IsComplete = false;
            state.ChildRunner = new GraphRunner(m_subGraph, context.GraphProperties);
            state.ChildRunner.OnGraphStop += () => state.IsComplete = true;
            state.ChildRunner.StartGraph();

            if (!state.ChildRunner.IsRunning)
            {
                state.IsComplete = true;
            }
        }

        public override void OnNodeUpdate(NodeExecutionContext context)
        {
            SubGraphNodeRuntimeState state = context.GetState<SubGraphNodeRuntimeState>();
            if (state?.ChildRunner == null)
            {
                context.StopGraph();
                return;
            }

            if (!state.IsComplete && state.ChildRunner.IsRunning)
            {
                state.ChildRunner.UpdateGraph();
            }

            if (state.IsComplete || !state.ChildRunner.IsRunning)
            {
                state.IsComplete = true;
                context.TraverseEdge(m_onCompleteOutportIndex);
            }
        }

        public override void OnNodeExit(NodeExecutionContext context)
        {
            SubGraphNodeRuntimeState state = context.GetState<SubGraphNodeRuntimeState>();
            if (state?.ChildRunner != null)
            {
                state.ChildRunner.StopGraph();
                state.ChildRunner = null;
                state.IsComplete = false;
            }
        }
    }

    public sealed class SubGraphNodeRuntimeState : NodeRuntimeState
    {
        public GraphRunner ChildRunner;
        public bool IsComplete;
    }
}

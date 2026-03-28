using System;
using UnityEngine;

namespace Logical99.Runtime
{
    [Serializable]
    public abstract class ANodeGraph : ScriptableObject
    {
        [SerializeField]
        private NodeCollection m_rootCollection;
        public NodeCollection NodeCollection => m_rootCollection;

        [SerializeReference]
        private AGraphProperties m_graphProperties;
        public AGraphProperties GraphProperties => m_graphProperties;
    }
}
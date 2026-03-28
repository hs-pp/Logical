using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logical.Editor
{
    [Serializable]
    public sealed class GraphEditorData
    {
        public Vector2 Pan;
        public float Zoom = 1f;
        public List<NodeEditorData> Nodes = new();
        public List<CommentEditorData> Comments = new();
    }

    [Serializable]
    public sealed class NodeEditorData
    {
        public string NodeId;
        public Vector2 Position;
        public Color Color;
    }

    [Serializable]
    public sealed class CommentEditorData
    {
        public string Id;
        public Rect Rect;
        public string Title;
    }
}
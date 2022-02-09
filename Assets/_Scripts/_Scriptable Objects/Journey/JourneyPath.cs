using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyPath : BaseScriptableObject // needed for instantiate
{
    public List<JourneyNode> nodes = new();
    public List<JourneyBridge> bridges = new();

    public void CreatePath(int _numberOfRows, JourneyNode[] _nodes, JourneyPathConfig[] _configs)
    {
        for (int i = 0; i < _numberOfRows; i++)
        {
            JourneyNode nodeToInstantiate = null;
            foreach (JourneyPathConfig c in _configs)
                if (c.nodeIndexRange.x <= i && c.nodeIndexRange.y > i && Random.Range(0f, 1f) > c.chanceToIgnore)
                    nodeToInstantiate = c.node;

            if (nodeToInstantiate == null)
                nodeToInstantiate = _nodes[Random.Range(0, _nodes.Length)];

            JourneyNode n = Instantiate(nodeToInstantiate);
            nodes.Add(n);
        }
    }
}

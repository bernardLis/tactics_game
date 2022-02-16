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
                    nodeToInstantiate = c.nodes[Random.Range(0, c.nodes.Length)];

            if (nodeToInstantiate == null)
                nodeToInstantiate = _nodes[Random.Range(0, _nodes.Length)];

            JourneyNode n = Instantiate(nodeToInstantiate);
            nodes.Add(n);
        }
    }

    public bool IsLegalMove(JourneyNode _current, JourneyNode _next)
    {
        if (nodes.IndexOf(_current) == -1) // not on path
            return false;

        // next node on path
        if (nodes.IndexOf(_next) == nodes.IndexOf(_current) + 1)
            return true;

        return false;
    }

    public JourneyNode CheckBridge(JourneyNode _current)
    {
        foreach (JourneyBridge b in bridges)
            if (b.from == _current)
                return b.to;
        return null;
    }
}

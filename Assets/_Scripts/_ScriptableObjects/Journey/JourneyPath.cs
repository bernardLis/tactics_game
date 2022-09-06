using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyPath : BaseScriptableObject // needed for instantiate
{
    public List<JourneyNode> Nodes = new();
    public List<JourneyBridge> Bridges = new();
    [HideInInspector] public int PathIndex;

    public void CreatePath(int numberOfRows, JourneyNode[] nodes, JourneyPathConfig[] configs)
    {
        for (int i = 0; i < numberOfRows; i++)
        {
            JourneyNode nodeToInstantiate = null;
            foreach (JourneyPathConfig c in configs)
                if (c.NodeIndexRange.x <= i && c.NodeIndexRange.y > i && Random.Range(0f, 1f) > c.ChanceToIgnore)
                    nodeToInstantiate = c.Nodes[Random.Range(0, c.Nodes.Length)];

            if (nodeToInstantiate == null)
                nodeToInstantiate = nodes[Random.Range(0, nodes.Length)];

            JourneyNode n = Instantiate(nodeToInstantiate);
            n.PathIndex = PathIndex;
            n.NodeIndex = i;
            Nodes.Add(n);
        }
    }

    public bool IsLegalMove(JourneyNode current, JourneyNode next)
    {
        if (Nodes.IndexOf(current) == -1) // not on path
            return false;

        // next node on path
        if (Nodes.IndexOf(next) == Nodes.IndexOf(current) + 1)
            return true;

        return false;
    }

    public JourneyNode CheckBridge(JourneyNode current)
    {
        foreach (JourneyBridge b in Bridges)
            if (b.FromNode == current)
                return b.ToNode;
        return null;
    }
}

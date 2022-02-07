using System.Collections.Generic;
using Random = UnityEngine.Random;

public class JourneyRow
{

    public List<JourneyNode> journeyNodes = new();

    public void CreateRow(int _numberOfNodes, JourneyNode[] _nodes) // TODO: should I pass on seed?
    {
        for (int i = 0; i < _numberOfNodes; i++)
        {
            JourneyNode n = _nodes[Random.Range(0, _nodes.Length)];
            journeyNodes.Add(n);
        }
    }

    public void RemoveRandomNodes()
    {
        List<JourneyNode> copy = new List<JourneyNode>(journeyNodes);
        foreach (JourneyNode n in copy)
            if (Random.Range(0f, 1f) < 0.2f)
                journeyNodes.Remove(n);
    }

    public void AddNode(JourneyNode _n)
    {
        journeyNodes.Add(_n);
    }

    public void RemoveNode(JourneyNode _n)
    {
        journeyNodes.Remove(_n);
    }
}

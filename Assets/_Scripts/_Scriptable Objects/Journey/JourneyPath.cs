using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyPath : BaseScriptableObject // needed for instantiate
{
    public List<JourneyNode> nodes = new();
    public List<JourneyBridge> bridges = new();

    public void CreatePath(int _numberOfRows, JourneyNode[] _nodes)
    {
        for (int i = 0; i < _numberOfRows; i++)
        {
            if (Random.Range(0f, 1f) > 0.2f) // 20% to skip a node // TODO: magic number
            {
                JourneyNode n = Instantiate(_nodes[Random.Range(0, _nodes.Length)]);
                nodes.Add(n);
            }
        }
    }
}

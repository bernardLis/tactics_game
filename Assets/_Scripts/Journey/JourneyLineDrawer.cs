using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class JourneyLineDrawer : ImmediateModeShapeDrawer
{
    List<JourneyPath> _paths;
    JourneyNode _startNode;
    JourneyNode _endNode;
    public void SetPaths(List<JourneyPath> paths, JourneyNode startNode, JourneyNode endNode)
    {
        _paths = new(paths);
        _startNode = startNode;
        _endNode = endNode;
    }

    public override void DrawShapes(Camera cam)
    {
        if (_paths == null || _paths.Count == 0)
            return;

        // start gets line renderer per path and renders a line
        foreach (JourneyPath p in _paths)
        {
            for (int i = -1; i < p.Nodes.Count; i++)
            {
                Vector3 offset = new Vector3(10f, 10f, 0);
                JourneyNode startNode = null;
                JourneyNode endNode = null;
                if (i == -1)
                {
                    startNode = _startNode;
                    endNode = p.Nodes[i + 1];
                }
                if (i + 1 == p.Nodes.Count)
                {
                    startNode = p.Nodes[i];
                    endNode = _endNode;
                }

                // TODO: probably risky way of doing things.
                if (startNode == null)
                    startNode = p.Nodes[i];

                if (endNode == null)
                    endNode = p.Nodes[i + 1];

                Color pathColor = Color.black;

                if (endNode.WasVisited)
                    pathColor = Color.blue;

                //offset
                Vector3 startPosition = startNode.GameObject.transform.position;
                Vector3 endPosition = endNode.GameObject.transform.position;
                startPosition += new Vector3(0, 5, 0);
                endPosition += new Vector3(0, -5, 0);

                using (Draw.Command(cam))
                {
                    Draw.DashSnap = DashSnapping.Tiling;
                    Draw.UseDashes = true;
                    Draw.Line(startPosition, endPosition, 1f, pathColor);
                }
            }
        }

        foreach (JourneyPath p in _paths)
        {
            foreach (JourneyBridge b in p.Bridges)
            {
                JourneyNode fromNode = b.FromNode;
                JourneyNode toNode = b.ToNode;

                Color pathColor = Color.black;

                if (toNode.WasVisited)
                    pathColor = Color.blue;

                //offset
                Vector3 startPosition = fromNode.GameObject.transform.position;
                Vector3 endPosition = toNode.GameObject.transform.position;
                startPosition += new Vector3(0, 5, 0);
                endPosition += new Vector3(0, -5, 0);

                using (Draw.Command(cam))
                {
                    Draw.DashSnap = DashSnapping.Tiling;
                    Draw.UseDashes = true;
                    Draw.Line(startPosition, endPosition, 1f, pathColor);
                }
            }
        }
    }
}

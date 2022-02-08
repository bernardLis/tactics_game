using System.Collections.Generic;
using UnityEngine;

public class JourneyConnection
{
    public JourneyNode from;
    public JourneyNode to;

    LineRenderer lineRenderer;

    public void CreateConnection(JourneyNode _from, JourneyNode _to)
    {
        from = _from;
        to = _to;

        GameObject g = new GameObject();
        g.transform.parent = _to.self.transform;
        lineRenderer = g.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.SetPosition(0, _from.self.transform.position);
        lineRenderer.SetPosition(1, _to.self.transform.position);
    }

    public void RemoveConnection()
    {
        // TODO: not like that.
        lineRenderer.startWidth = 0;

    }
}

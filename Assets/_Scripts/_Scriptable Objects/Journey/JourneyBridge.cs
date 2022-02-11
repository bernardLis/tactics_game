using UnityEngine;
public class JourneyBridge
{
    public JourneyNode from;
    public JourneyNode to;

    public void Initialize(JourneyNode _from, JourneyNode _to)
    {
        from = _from;
        to = _to;
                
        // from gets a line renderer
        GameObject g = new GameObject();
        g.transform.parent = from.gameObject.transform;
        LineRenderer lr = g.AddComponent<LineRenderer>();
        lr.startWidth = 0.2f;
        lr.SetPosition(0, from.gameObject.transform.position);
        lr.SetPosition(1, to.gameObject.transform.position);
    }



}

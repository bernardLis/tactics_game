using UnityEngine;
public class JourneyBridge
{
    public JourneyNode From;
    public JourneyNode To;

    public void Initialize(JourneyNode from, JourneyNode to, Material mat)
    {
        From = from;
        To = to;

        // from gets a line renderer
        GameObject g = new GameObject();
        g.transform.parent = From.GameObject.transform;
        LineRenderer lr = g.AddComponent<LineRenderer>();
        lr.material = mat;
        lr.textureMode = LineTextureMode.Tile;

        lr.SetPosition(0, From.GameObject.transform.position);
        lr.SetPosition(1, To.GameObject.transform.position);
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class ShapesPainter : ImmediateModeShapeDrawer
{
    Vector3 _startPosLine;
    Vector3 _endPosLine;

    PolylinePath l;

    public void DrawLine(Vector3 startPos, Vector3 endPos)
    {
        l = new PolylinePath();
        l.AddPoint(startPos.x, startPos.y);
        l.AddPoint(endPos.x, endPos.y);

        DrawShapes(Camera.main);
    }

    public void ClearLine()
    {
        l = new PolylinePath();
    }

    public override void DrawShapes(Camera cam)
    {
        if (l != null)
            using (Draw.Command(cam)) { Draw.Polyline(l, thickness: 0.05f, Color.red); }

        using (Draw.Command(cam))
        {
            // set up static parameters. these are used for all following Draw.Line calls
            Draw.LineGeometry = LineGeometry.Volumetric3D;
            Draw.ThicknessSpace = ThicknessSpace.Pixels;
            Draw.Thickness = 4; // 4px wide

            // set static parameter to draw in the local space of this object
            Draw.Matrix = transform.localToWorldMatrix;

            // draw lines
            Draw.Line(Vector3.zero, Vector3.right, Color.red);
            Draw.Line(Vector3.zero, Vector3.up, Color.green);
            Draw.Line(Vector3.zero, Vector3.forward, Color.blue);
        }
    }
}

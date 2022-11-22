using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class ShapesPainter : ImmediateModeShapeDrawer
{
    Vector3 _startPosLine;
    Vector3 _endPosLine;

    bool _isLineBeingDrawn;
    Vector3 _start;
    Vector3 _end;

    public void DrawLine(Vector3 startPos, Vector3 endPos)
    {
        _start = startPos;
        _end = endPos;
        _isLineBeingDrawn = true;

        DrawShapes(Camera.main);
    }

    public void ClearLine()
    {
        _isLineBeingDrawn = false;
    }

    public override void DrawShapes(Camera cam)
    {
        if (_isLineBeingDrawn)
            using (Draw.Command(cam))
            {
                Draw.UseDashes = true;
                Draw.DashSize = 1;
                Draw.DashSpacing = 1;
                Draw.Line(_start, _end, thickness: 0.05f, color: Color.white);
            }
    }
}

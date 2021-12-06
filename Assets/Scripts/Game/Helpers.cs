using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    private static Camera _camera;
    //https://www.youtube.com/watch?v=JOABOQMurZo
    public static Camera Camera
    {
        get
        {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }

    public static int GetManhattanDistance(Vector2 _start, Vector2 _end)
    {
        float xDist = Mathf.Abs(_start.x - _end.x);
        float yDist = Mathf.Abs(_start.y - _end.y);

        return Mathf.RoundToInt(xDist + yDist);
    }

}

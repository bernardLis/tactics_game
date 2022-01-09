using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{

    static Dictionary<string, Color> colors = new()
    {
        { "movementBlue", new Color(0.53f, 0.52f, 1f, 1f) },
        { "healthGainGreen", new Color(0.42f, 1f, 0.42f, 1f) },
        { "damageRed", new Color(1f, 0.42f, 0.42f, 1f) },


    };

    static Camera _camera;
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
        return Mathf.RoundToInt(Mathf.Abs(_start.x - _end.x) + Mathf.Abs(_start.y - _end.y));
    }

    public static Color GetColor(string _name)
    {
        Color col;
        if (!colors.TryGetValue(_name, out col))
        {
            // the key isn't in the dictionary.
            return Color.black; // or whatever you want to do
        }
        return col;
    }

}

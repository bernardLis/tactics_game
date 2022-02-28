using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    static Dictionary<string, Color> _colors = new()
    {
        { "movementBlue", new Color(0.53f, 0.52f, 1f, 1f) },
        { "healthGainGreen", new Color(0.42f, 1f, 0.42f, 1f) },
        { "damageRed", new Color(1f, 0.42f, 0.42f, 1f) },
        { "gray", new Color(0.22f, 0.22f, 0.22f, 1f) }
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

    public static int GetManhattanDistance(Vector2 start, Vector2 end)
    {
        return Mathf.RoundToInt(Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y));
    }

    public static Color GetColor(string name)
    {
        Color col;
        if (!_colors.TryGetValue(name, out col))
        {
            Debug.LogError($"Color: {name} is not in the color dictionary");
            return Color.black;
        }
        return col;
    }

}

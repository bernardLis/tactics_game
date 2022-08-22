using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class Helpers
{
    static Dictionary<string, Color> _colors = new()
    {
        { "healthBarRed", new Color(0.529f, 0.16f, 0.16f) },
        { "manaBarBlue", new Color(0.168f, 0.149f, 0.85f) },
        { "movementBlue", new Color(0.53f, 0.52f, 1f, 1f) },
        { "healthGainGreen", new Color(0.42f, 1f, 0.42f, 1f) },
        { "damageRed", new Color(1f, 0.42f, 0.42f, 1f) },
        { "gray", new Color(0.22f, 0.22f, 0.22f, 1f) },
        { ItemRaririty.Common.ToString(), new Color(1f,1f,1f,1f) },
        { ItemRaririty.Magic.ToString(), new Color(0.31f,1f,0.69f,1f) },
        { ItemRaririty.Rare.ToString(), new Color(0.38f,0.51f,0.84f,1f) },
        { ItemRaririty.Epic.ToString(), new Color(0.32f,0.22f,0.44f,1f) },
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

    public static string ParseScriptableObjectCloneName(string text)
    {
        text = text.Replace("(Clone)", "");
        // https://stackoverflow.com/questions/3216085/split-a-pascalcase-string-into-separate-words
        Regex r = new Regex(
            @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])"
          );

        text = r.Replace(text, " "); // pascal case
        text = Regex.Replace(text, @"\s+", " "); // whitespace clean-up
        return text;
    }

}

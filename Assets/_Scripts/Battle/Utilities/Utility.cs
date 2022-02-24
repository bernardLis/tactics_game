using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    //https://www.youtube.com/watch?v=q7BL-lboRXo&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=10
    public static List<T> ShuffleList<T>(List<T> _list, int _seed)
    {
        System.Random prng = new(_seed);

        for (int i = 0; i < _list.Count - 1; i++)
        {
            int randomIndex = prng.Next(i, _list.Count);
            T tempItem = _list[randomIndex];
            _list[randomIndex] = _list[i];
            _list[i] = tempItem;
        }
        return _list;
    }

    public static Color HexToColor(string hex)
    {
        // I want to support #aaa, #AAA, aaa, AAA
        if (hex[0].ToString() != "#")
            hex = "#" + hex;

        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
            return color;
    
        Debug.LogError($"Couldn't parse color: {hex}");
        return Color.black;
    }
}

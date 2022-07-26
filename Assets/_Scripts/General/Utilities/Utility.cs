using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    //https://www.youtube.com/watch?v=q7BL-lboRXo&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=10
    public static List<T> ShuffleList<T>(List<T> list, int seed)
    {
        System.Random prng = new(seed);

        for (int i = 0; i < list.Count - 1; i++)
        {
            int randomIndex = prng.Next(i, list.Count);
            T tempItem = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = tempItem;
        }
        return list;
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

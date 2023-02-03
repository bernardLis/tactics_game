using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#if UNITY_EDITOR

// https://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/
public class CSVReader
{
    static string _splitCharacters = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string _lineBreakCharacters = @"\r\n|\n\r|\n|\r";
    static char[] _trimCharacters = { '\"' };

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        //TextAsset data = Resources.Load("GameData") as TextAsset;
        //TextAsset data = (TextAsset)Resources.Load(file, typeof(TextAsset));
        string rawText = System.IO.File.ReadAllText(file);

        var lines = Regex.Split(rawText, _lineBreakCharacters);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], _splitCharacters);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], _splitCharacters);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(_trimCharacters).TrimEnd(_trimCharacters).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }
}

#endif

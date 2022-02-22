using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JourneyMapManager))]
public class TempCharacterManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TempCharacterManager n = target as TempCharacterManager;

        //if (DrawDefaultInspector())
        //    board.GenerateMap();

        if (GUILayout.Button("Characters to resources"))
            n.CharactersToResources();

        //board.GenerateMap();
    }


}

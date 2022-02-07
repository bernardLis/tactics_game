using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JourneyMapManager))]
public class JourneyMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        JourneyMapManager n = target as JourneyMapManager;

        //if (DrawDefaultInspector())
        //    board.GenerateMap();

        if (GUILayout.Button("Generate Journey"))
            n.GenerateJourney();

        //board.GenerateMap();
    }


}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JourneyManager))]
public class JourneyManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        JourneyManager n = target as JourneyManager;

        if (GUILayout.Button("Clear Save Data"))
            n.ClearSaveData();
    }


}

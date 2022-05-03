using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class JourneyManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameManager n = target as GameManager;

        if (GUILayout.Button("Clear Save Data"))
            n.ClearSaveData();
    }


}

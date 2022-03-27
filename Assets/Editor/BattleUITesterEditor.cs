using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleUITester))]
public class BattleUITesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BattleUITester n = target as BattleUITester;

        if (GUILayout.Button("Show Battle Won Screen"))
            n.ShowBattleWonScreen();
    }


}

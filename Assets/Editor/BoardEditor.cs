using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoardManager))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BoardManager board = target as BoardManager;
        //if (DrawDefaultInspector())
       // {
       //     board.SetupScene();
       // }

        //if (GUILayout.Button("Generate Map"))
        //{
       //     board.SetupScene();
       // }

        board.SetupScene();
    }
}

using System;
using UnityEditor;
using UnityEngine;

namespace Lis
{
    // https://stackoverflow.com/questions/58984486/create-scriptable-object-with-constant-unique-id

    public class ScriptableObjectIdAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
    public class ScriptableObjectIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // GUI.enabled = false;
            if (string.IsNullOrEmpty(property.stringValue))
            {
                property.stringValue = Guid.NewGuid().ToString();
            }
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;

            if (GUILayout.Button("Generate new GUID"))
            {
                property.stringValue = Guid.NewGuid().ToString();
            }
        }
    }
#endif

    public class BaseScriptableObject : ScriptableObject
    {
        [ScriptableObjectId]
        public string Id;
    }
}
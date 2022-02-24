using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.IO;
using UnityEditor;

public static class SOGeneratorUtility
{
    public static StatModifier GetStatModifierFromReferenceId(string id)
    {
        Debug.Log("GetStatModifierFromReferenceId");
        string path = "Assets/Resources/Abilities/StatModifiers/ArmorUp.asset";
        Object[] data = AssetDatabase.LoadAllAssetsAtPath(path);

        Debug.Log(data.Length + " Assets");

        foreach (Object o in data)
        {
            Debug.Log(o);
        }

        /*
        Debug.Log($"mods.length {mods.Length}");
        foreach (StatModifier mod in mods)
            if (mod.ReferenceID == id)
            {
                Debug.Log("returniung " + mod);
                return mod;

            }
            */


        Debug.LogError($"did not find a stat modifier with id: {id}");
        return null;
    }
}

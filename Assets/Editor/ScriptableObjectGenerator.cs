using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEditor;
using System.IO;

public static class ScriptableObjectGenerator
{
    [MenuItem("Utilities/Generate Abilities")]
    public static void GenerateAbilities()
    {
        string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(Selection.activeObject));

        if (!IsSelectionCSV(path))
            return;

        List<Dictionary<string, object>> rawCSVData = CSVReader.Read(path);
        if (rawCSVData.Count <= 0)
        {
            Debug.LogError("no data in the file");
            return;
        }

        foreach (Dictionary<string, object> item in rawCSVData)
        {
            if (item["SOName"].ToString().Length == 0)
                continue;

            CreateAbility(item);
        }
    }

    [MenuItem("Utilities/Generate Stat Modifiers")]
    public static void GenerateStatModifiers()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(Selection.activeObject));

        if (!IsSelectionCSV(path))
            return;

        List<Dictionary<string, object>> rawCSVData = CSVReader.Read(path);
        if (rawCSVData.Count <= 0)
        {
            Debug.LogError("no data in the file");
            return;
        }

        foreach (Dictionary<string, object> item in rawCSVData)
        {
            if (item["SOName"].ToString().Length == 0)
                continue;

            CreateStatModifier(item);
        }
    }

    static void CreateAbility(Dictionary<string, object> item)
    {
        // https://stackoverflow.com/questions/64056647/making-a-tool-to-edit-scriptableobjects
        // TODO: Need to check which ability it is and instantiate a proper one
        Ability ability = null;
        //Attack, Heal, Move, Buff, Utility }
        if (item["AbilityType"].ToString() == "Attack")
            ability = (AttackAbility)ScriptableObject.CreateInstance<AttackAbility>();
        if (item["AbilityType"].ToString() == "Heal")
            ability = (HealAbility)ScriptableObject.CreateInstance<HealAbility>();
        if (item["AbilityType"].ToString() == "Push")
            ability = (PushAbility)ScriptableObject.CreateInstance<PushAbility>();
        if (item["AbilityType"].ToString() == "Buff")
            ability = (BuffAbility)ScriptableObject.CreateInstance<BuffAbility>();
        if (item["AbilityType"].ToString() == "Utility")
            ability = (UtilityAbility)ScriptableObject.CreateInstance<UtilityAbility>();

        if (ability == null)
        {
            Debug.LogError($"Wrong ability type: {item["AbilityType"]}");
            return;
        }

        string path = $"Assets/Resources/Abilities/{item["SOName"]}.asset";
        AssetDatabase.CreateAsset(ability, path);
        ability.Create(item);

        // Now flag the object as "dirty" in the editor so it will be saved
        EditorUtility.SetDirty(ability);
        // And finally, prompt the editor database to save dirty assets, committing your changes to disk.
        AssetDatabase.SaveAssets();
    }

    static void CreateStatModifier(Dictionary<string, object> item)
    {
        string path = $"Assets/Resources/Abilities/StatModifiers/{item["SOName"]}.asset";
        StatModifier mod = (StatModifier)ScriptableObject.CreateInstance<StatModifier>();
        AssetDatabase.CreateAsset(mod, path);
        mod.Create(item);

        // Now flag the object as "dirty" in the editor so it will be saved
        EditorUtility.SetDirty(mod);
        // And finally, prompt the editor database to save dirty assets, committing your changes to disk.
        AssetDatabase.SaveAssets();
    }

    static bool IsSelectionCSV(string path)
    {
        if (Selection.activeObject == null)
        {
            Debug.LogError("no selection");
            return false;
        }

        if (!IsCSVFile(path))
        {
            Debug.LogError("Not a CSV file");
            return false;
        }
        return true;
    }

    static bool IsCSVFile(string fullPath)
    {
        return fullPath.ToLower().EndsWith(".csv");
    }

}

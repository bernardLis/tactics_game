using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [MenuItem("Utilities/Generate Statuses")]
    public static void GenerateStatuses()
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

            CreateStatus(item);
        }
    }


    static void CreateAbility(Dictionary<string, object> item)
    {
        // https://stackoverflow.com/questions/64056647/making-a-tool-to-edit-scriptableobjects
        Ability ability = null;
        switch (item["AbilityType"].ToString())
        {
            case "Attack":
                ability = (AttackAbility)ScriptableObject.CreateInstance<AttackAbility>();
                break;
            case "Heal":
                ability = (HealAbility)ScriptableObject.CreateInstance<HealAbility>();
                break;
            case "Push":
                ability = (PushAbility)ScriptableObject.CreateInstance<PushAbility>();
                break;
            case "Buff":
                ability = (BuffAbility)ScriptableObject.CreateInstance<BuffAbility>();
                break;
            case "Utility":
                ability = (UtilityAbility)ScriptableObject.CreateInstance<UtilityAbility>();
                break;
            default:
                Debug.LogError($"Wrong Ability Type {item["AbilityType"].ToString()}");
                break;
        }

        if (ability == null)
        {
            Debug.LogError($"Wrong ability type: {item["AbilityType"]}");
            return;
        }

        string path = $"Assets/Resources/Abilities/{item["SOName"]}.asset";
        AssetDatabase.CreateAsset(ability, path);

        StatModifier statModifier = null;
        if (item["StatModifierReferenceID"].ToString() != "")
            statModifier = GetStatModifierFromReferenceId(item["StatModifierReferenceID"].ToString()) as StatModifier;

        Status status = null;
        if (item["StatusReferenceID"].ToString() != "")
            status = GetStatusFromReferenceID(item["StatusReferenceID"].ToString()) as Status;

        ability.Create(item, statModifier, status);

        // Now flag the object as "dirty" in the editor so it will be saved
        EditorUtility.SetDirty(ability);
        // And finally, prompt the editor database to save dirty assets, committing your changes to disk.
        AssetDatabase.SaveAssets();
    }

    static void CreateStatus(Dictionary<string, object> item)
    {
        // https://stackoverflow.com/questions/64056647/making-a-tool-to-edit-scriptableobjects
        // TODO: Need to check which ability it is and instantiate a proper one
        Status status = null;
        switch (item["StatusType"].ToString())
        {
            case "Damage":
                status = (DamageStatus)ScriptableObject.CreateInstance<DamageStatus>();
                break;
            case "Heal":
                status = (HealStatus)ScriptableObject.CreateInstance<HealStatus>();
                break;
            case "Stun":
                status = (StunStatus)ScriptableObject.CreateInstance<StunStatus>();
                break;
            default:
                Debug.LogError($"Wrong Ability Type {item["AbilityType"].ToString()}");
                break;
        }

        if (status == null)
        {
            Debug.LogError($"Wrong ability type: {item["StatusType"]}");
            return;
        }

        string path = $"Assets/Resources/Abilities/Statuses/{item["SOName"]}.asset";
        AssetDatabase.CreateAsset(status, path);

        status.Create(item);

        // Now flag the object as "dirty" in the editor so it will be saved
        EditorUtility.SetDirty(status);
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

    static StatModifier GetStatModifierFromReferenceId(string id)
    {
        string path = "Abilities/StatModifiers";
        Object[] mods = Resources.LoadAll(path, typeof(StatModifier));
        foreach (StatModifier mod in mods)
            if (mod.ReferenceID == id)
                return mod;

        Debug.LogError($"Did not find a stat modifier with id: {id}");
        return null;
    }

    static Status GetStatusFromReferenceID(string id)
    {
        string path = "Abilities/Statuses";
        Object[] statuses = Resources.LoadAll(path, typeof(Status));
        foreach (Status st in statuses)
            if (st.ReferenceID == id)
                return st;

        Debug.LogError($"Did not find a status with id: {id}");
        return null;
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

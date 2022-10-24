using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using Object = UnityEngine.Object;

public static class ScriptableObjectGenerator
{
    [MenuItem("Utilities/SOGenerator/Generate Portraits Male")]
    public static void GeneratePortraitsMale()
    {
        object[] loadedSprites = Resources.LoadAll("Portraits/male", typeof(Sprite));
        Sprite[] Portraits = new Sprite[loadedSprites.Length];
        loadedSprites.CopyTo(Portraits, 0);

        foreach (Sprite s in Portraits)
        {

            CharacterPortrait characterPortrait = ScriptableObject.CreateInstance<CharacterPortrait>();
            characterPortrait.Id = Guid.NewGuid().ToString();
            characterPortrait.Sprite = s;

            string path = $"Assets/Resources/Portraits/SO/male/{s.name}.asset";

            AssetDatabase.CreateAsset(characterPortrait, path);

            // Now flag the object as "dirty" in the editor so it will be saved
            EditorUtility.SetDirty(characterPortrait);
            // And finally, prompt the editor database to save dirty assets, committing your changes to disk.
            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("Utilities/SOGenerator/Generate Portraits Female")]
    public static void GeneratePortraitsFemale()
    {
        object[] loadedSprites = Resources.LoadAll("Portraits/female", typeof(Sprite));
        Sprite[] Portraits = new Sprite[loadedSprites.Length];
        loadedSprites.CopyTo(Portraits, 0);

        foreach (Sprite s in Portraits)
        {

            CharacterPortrait characterPortrait = ScriptableObject.CreateInstance<CharacterPortrait>();
            characterPortrait.Id = Guid.NewGuid().ToString();
            characterPortrait.Sprite = s;

            string path = $"Assets/Resources/Portraits/SO/female/{s.name}.asset";

            AssetDatabase.CreateAsset(characterPortrait, path);

            // Now flag the object as "dirty" in the editor so it will be saved
            EditorUtility.SetDirty(characterPortrait);
            // And finally, prompt the editor database to save dirty assets, committing your changes to disk.
            AssetDatabase.SaveAssets();
        }
    }




    [MenuItem("Utilities/SOGenerator/Generate Characters")]
    public static void GenerateCharacters()
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

            CreateCharacter(item);
        }
    }


    [MenuItem("Utilities/SOGenerator/Generate Abilities")]
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

    [MenuItem("Utilities/SOGenerator/Generate Stat Modifiers")]
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

    [MenuItem("Utilities/SOGenerator/Generate Statuses")]
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

    static void CreateCharacter(Dictionary<string, object> item)
    {
        Character character = ScriptableObject.CreateInstance<Character>();

        string path = $"Assets/Resources/Characters/{item["SOName"]}.asset";
        AssetDatabase.CreateAsset(character, path);

        // load abilities
        List<Ability> abilities = new();
        string[] arr = item["AbilitiesReferenceID"].ToString().Split(" ");
        foreach (string s in arr)
            abilities.Add(GetAbilityFromReferenceID(s));

        //character.CreateFromSheetData(item, abilities);

        // Now flag the object as "dirty" in the editor so it will be saved
        EditorUtility.SetDirty(character);
        // And finally, prompt the editor database to save dirty assets, committing your changes to disk.
        AssetDatabase.SaveAssets();
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

        //ability.Create(item, statModifier, status);

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

        //status.Create(item);

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
        //mod.Create(item);

        // Now flag the object as "dirty" in the editor so it will be saved
        EditorUtility.SetDirty(mod);
        // And finally, prompt the editor database to save dirty assets, committing your changes to disk.
        AssetDatabase.SaveAssets();
    }

    static Ability GetAbilityFromReferenceID(string id)
    {
        id = id.Trim();
        string path = "Abilities";
        Object[] abilities = Resources.LoadAll(path, typeof(Ability));
        foreach (Ability a in abilities)
            if (a.ReferenceID == id)
                return a;

        Debug.LogError($"Did not find a ability with id: {id}");
        return null;
    }

    static StatModifier GetStatModifierFromReferenceId(string id)
    {
        id = id.Trim();
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
        id = id.Trim();
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


/*Ability
    // called from editor using table data
    public virtual void Create(Dictionary<string, object> item, StatModifier statModifier, Status status)
    {
        ReferenceID = item["ReferenceID"].ToString();
        Description = item["Description"].ToString();
        AbilityType = (AbilityType)System.Enum.Parse(typeof(AbilityType), item["AbilityType"].ToString());
        WeaponType = (WeaponType)System.Enum.Parse(typeof(WeaponType), item["WeaponType"].ToString());
        Projectile = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Prefabs/Battle/Projectiles/{item["Projectile"]}.prefab", typeof(GameObject));
        AbilityEffect = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Prefabs/Battle/Effects/AbilityEffects/{item["AbilityEffect"]}.prefab", typeof(GameObject));
        Icon = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Sprites/Ability/{item["Icon"]}", typeof(Sprite));
        Sound = (Sound)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/General/Sounds/{item["Sound"]}.asset", typeof(Sound));
        BasePower = int.Parse(item["BasePower"].ToString());
        ManaCost = int.Parse(item["ManaCost"].ToString());
        AreaOfEffect = int.Parse(item["AreaOfEffect"].ToString());
        LineAreaOfEffect = item["LineAreaOfEffect"].ToString() == "TRUE" ? true : false;
        StatModifier = statModifier;
        Status = status;
        Range = int.Parse(item["Range"].ToString());
        CanTargetSelf = item["CanTargetSelf"].ToString() == "TRUE" ? true : false;
        CanTargetDiagonally = item["CanTargetDiagonally"].ToString() == "TRUE" ? true : false;
        HighlightColor = Utility.HexToColor(item["HighlightColor"].ToString());
    }

character
    // creates character from google sheet data in editor
    public virtual void CreateFromSheetData(Dictionary<string, object> item, List<Ability> abilities)
    {
        ReferenceID = item["ReferenceID"].ToString();
        CharacterName = item["CharacterName"].ToString();
        Portrait = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Sprites/Character/Portrait/{item["Portrait"]}", typeof(Sprite));

        Level = int.Parse(item["Level"].ToString());
        Experience = int.Parse(item["Experience"].ToString());
        Power = int.Parse(item["Power"].ToString());

        Body = (Equipment)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/Battle/_Scriptable Objects/Equipment/{item["Body"]}.asset", typeof(Equipment));
        Weapon = (Weapon)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/Battle/_Scriptable Objects/Equipment/Weapon/{item["Weapon"]}.asset", typeof(Weapon));
        BasicAbilities.Add((Ability)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/Battle/_Scriptable Objects/Abilities/BasicDefend.asset", typeof(Ability)));
        Abilities = new(abilities);
    }


*/
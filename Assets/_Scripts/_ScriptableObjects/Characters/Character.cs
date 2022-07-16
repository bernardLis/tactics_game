using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Player")]
public class Character : BaseScriptableObject
{
    // character scriptable object holds stats & abilities of a character.
    // it passes these values to CharacterStats script where they can be used in game.
    public string ReferenceID;
    public string CharacterName = "Default";
    public Sprite Portrait;

    [Header("Stats")]
    public int Level;
    public int Experience;

    public int Power;

    public int MaxHealth = 100;
    public int MaxMana = 50;
    public int Armor = 0;
    public int MovementRange = 4;

    [Header("Equipment")]
    public Equipment Body;
    public Weapon Weapon;

    [Header("Abilities")]
    [Tooltip("For now just defend, basic attack is from the weapon")]
    public List<Ability> BasicAbilities = new();
    public List<Ability> Abilities = new();

    public event Action OnCharacterLevelUp;
    public event Action<int> OnCharacterExpGain;

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

    // creates character at runtime from saved data
    public virtual void Create(CharacterData data)
    {
        CharacterDatabase CharacterDatabase = GameManager.Instance.CharacterDatabase;

        ReferenceID = data.ReferenceID;
        CharacterName = data.CharacterName;
        Portrait = CharacterDatabase.GetPortraitByID(data.Portrait);

        Level = data.Level;
        Experience = data.Experience;
        Power = data.Power;

        Body = CharacterDatabase.GetBodyByName(data.Body);
        Weapon = CharacterDatabase.GetWeaponByName(data.Weapon);

        BasicAbilities.Add(CharacterDatabase.GetAbilityByID("5f7d8c47-7ec1-4abf-b8ec-74ea82be327f")); // basic defend id

        foreach (string id in data.AbilityReferenceIds)
            Abilities.Add(CharacterDatabase.GetAbilityByReferenceID(id));
    }

    public virtual void Initialize(GameObject obj)
    {
        Transform bodyObj = obj.transform.Find("Body");
        if (Body != null)
            Body.Initialize(bodyObj.gameObject);

        Transform weaponObj = bodyObj.transform.Find("Weapon");
        if (Weapon != null)
            Weapon.Initialize(weaponObj.gameObject);
    }

    public int GetStatValue(string stat)
    {
        if (stat == "Power")
            return Power;
        if (stat == "MaxHealth")
            return MaxHealth;
        if (stat == "MaxMana")
            return MaxMana;
        if (stat == "Armor")
            return Armor;
        if (stat == "MovementRange")
            return MovementRange;

        return 0;
    }

    public void GetExp(int gain)
    {
        BaseExpGain(gain);
    }

    public void GetExp(GameObject target, bool isKill = false)
    {
        int exp = CalculateExpGain(target, isKill);
        BaseExpGain(exp);
    }

    protected virtual void BaseExpGain(int gain)
    {
        Experience += gain;
        OnCharacterExpGain?.Invoke(gain);

        if (Experience < 100)
            return;

        LevelUp();
    }

    int CalculateExpGain(GameObject target, bool isKill)
    {
        int expGain = 10;
        if (isKill)
            expGain += 20;

        int targetLevel = target.GetComponent<CharacterStats>().Character.Level;
        int levelExpGain = (targetLevel - Level) * 6;
        levelExpGain = Mathf.Clamp(levelExpGain, 0, 100);
        expGain += levelExpGain;

        return expGain;
    }

    public void LevelUp()
    {
        Experience = 0;

        Level++;
        Power += Random.Range(0, 2); ;

        OnCharacterLevelUp?.Invoke();
    }
}

[System.Serializable]
public struct CharacterData
{
    public string ReferenceID;
    public string CharacterName;
    public string Portrait;
    public int Level;
    public int Experience;
    public int Power;
    public string Body;
    public string Weapon;
    public List<string> AbilityReferenceIds;
}

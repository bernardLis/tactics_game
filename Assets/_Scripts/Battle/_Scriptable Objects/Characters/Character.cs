using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
    public int Strength; // how strong you hit
    public int Intelligence; // maxMana depends on intelligence (also how strong the spell dmg is)
    public int Agility; // influences range
    public int Stamina; // influences maxHealth

    [Header("Equipment")]
    public Equipment Body;
    public Weapon Weapon;

    [Header("Abilities")]
    [Tooltip("For now just defend, basic attack is from the weapon")]
    public List<Ability> BasicAbilities = new();
    public List<Ability> Abilities = new();

    public virtual void Create(Dictionary<string, object> item, List<Ability> abilities)
    {
        ReferenceID = item["ReferenceID"].ToString();
        CharacterName = item["CharacterName"].ToString();
        Portrait = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Sprites/Character/Portrait/{item["Portrait"]}", typeof(Sprite));
        Level = int.Parse(item["Strength"].ToString());
        Strength = int.Parse(item["Strength"].ToString());
        Intelligence = int.Parse(item["Intelligence"].ToString());
        Agility = int.Parse(item["Agility"].ToString());
        Stamina = int.Parse(item["Stamina"].ToString());
        Body = (Equipment)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/Battle/_Scriptable Objects/Equipment/{item["Body"]}.asset", typeof(Equipment));
        Weapon = (Weapon)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/Battle/_Scriptable Objects/Equipment/Weapon/{item["Weapon"]}.asset", typeof(Weapon));
        BasicAbilities.Add((Ability)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/Battle/_Scriptable Objects/Abilities/BasicDefend.asset", typeof(Ability)));
        Abilities = new(abilities);
    }

    public virtual void Create(CharacterData data)
    {
        ReferenceID = data.ReferenceID;
        CharacterName = data.CharacterName;
        Portrait = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Sprites/Character/Portrait/{data.Portrait}", typeof(Sprite));
        Level = data.Level;
        Strength = data.Strength;
        Intelligence = data.Intelligence;
        Agility = data.Agility;
        Stamina = data.Stamina;
        Body = (Equipment)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/Battle/_Scriptable Objects/Equipment/{data.Body}", typeof(Equipment));
        Weapon = (Weapon)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/Battle/_Scriptable Objects/Equipment/Weapon/{data.Weapon}", typeof(Weapon));
        BasicAbilities.Add((Ability)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/Battle/_Scriptable Objects/Abilities/BasicDefend.asset", typeof(Ability)));

        string path = "Abilities";
        Object[] loadedAbilities = Resources.LoadAll(path, typeof(Ability));
        foreach (string id in data.AbilityReferenceIds)
            foreach (Ability loadedAbility in loadedAbilities)
                if (id == loadedAbility.ReferenceID)
                    Abilities.Add(loadedAbility);
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
}

[System.Serializable]
public struct CharacterData
{
    public string ReferenceID;
    public string CharacterName;
    public string Portrait;
    public int Level;
    public int Strength;
    public int Intelligence;
    public int Agility;
    public int Stamina;
    public string Body;
    public string Weapon;
    public List<string> AbilityReferenceIds;
}

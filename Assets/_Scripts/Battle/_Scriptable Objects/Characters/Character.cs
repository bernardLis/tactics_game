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

    int _baseMaxHealth = 100;
    int _baseMaxMana = 50;
    int _baseArmor = 0;
    int _baseMovementRange = 3;

    public int MaxHealth;
    public int MaxMana;
    public int Armor;
    public int MovementRange;

    [Header("Equipment")]
    public Equipment Body;
    public Weapon Weapon;

    [Header("Abilities")]
    [Tooltip("For now just defend, basic attack is from the weapon")]
    public List<Ability> BasicAbilities = new();
    public List<Ability> Abilities = new();

    // creates character from google sheet data in editor
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

    // creates character at runtime from saved data
    public virtual void Create(CharacterData data)
    {
        CharacterDatabase CharacterDatabase = JourneyManager.Instance.CharacterDatabase;

        ReferenceID = data.ReferenceID;
        CharacterName = data.CharacterName;
        Portrait = CharacterDatabase.GetPortraitByID(data.Portrait);

        Level = data.Level;
        Strength = data.Strength;
        Intelligence = data.Intelligence;
        Agility = data.Agility;
        Stamina = data.Stamina;

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

    public void UpdateDerivativeStats()
    {
        MaxHealth = _baseMaxHealth + Stamina * 5;
        MaxMana = _baseMaxMana + Intelligence * 5;
        Armor = _baseArmor; // TODO: should take eq into consideration 
        MovementRange = Mathf.Clamp(_baseMovementRange + Mathf.FloorToInt(Agility / 3), 1, 10);
    }

    public void LevelUp()
    {
        Level++;
        Strength += Random.Range(0, 2);
        Intelligence += Random.Range(0, 2); ;
        Agility += Random.Range(0, 2);
        Stamina += Random.Range(0, 2);
        UpdateDerivativeStats();
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

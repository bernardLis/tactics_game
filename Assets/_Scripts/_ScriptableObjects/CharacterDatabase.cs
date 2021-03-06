using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterDatabase")]
public class CharacterDatabase : BaseScriptableObject
{
    public PortraitEntry[] PortraitDatabase;
    public Equipment[] BodyDatabase;
    public Weapon[] WeaponDatabase;
    public Ability[] AbilityDatabase;
    public StatIcon[] StatIcons;

    public Sprite GetPortraitByID(string id)
    {
        return PortraitDatabase.FirstOrDefault(x => x.ReferenceID == id).Sprite;
    }

    public Equipment GetBodyByName(string name)
    {
        return BodyDatabase.FirstOrDefault(x => x.name == name);
    }

    public Weapon GetWeaponByName(string name)
    {
        return WeaponDatabase.FirstOrDefault(x => x.name == name);
    }

    public Ability GetAbilityByID(string id)
    {
        return AbilityDatabase.FirstOrDefault(x => x.Id == id);
    }

    public Ability GetAbilityByReferenceID(string id) // TODO: I am not certain if this reference ID and normal ID is a smart move.
    {
        return AbilityDatabase.FirstOrDefault(x => x.ReferenceID == id);
    }

    public Sprite GetStatIconByName(string name) // TODO: I am not certain if this reference ID and normal ID is a smart move.
    {
        return StatIcons.FirstOrDefault(x => x.StatName == name).Sprite;
    }

}

[System.Serializable]
public struct PortraitEntry
{
    public string ReferenceID;
    public Sprite Sprite;
}

[System.Serializable]
public struct StatIcon
{
    public string StatName;
    public Sprite Sprite;
}


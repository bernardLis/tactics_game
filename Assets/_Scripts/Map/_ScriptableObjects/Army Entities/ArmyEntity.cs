using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Army Entity")]
public class ArmyEntity : BaseScriptableObject
{
    public Sprite Icon;
    public int Price;

    public List<GameObject> GFX = new();
    public Material Material;
    public float Health;

    public float Power;
    public float AttackRange;
    public float AttackCooldown;

    public float Speed;

    public GameObject Projectile;

    public ArmyEntityData SerializeSelf()
    {
        ArmyEntityData data = new();
        data.Id = Id;
        return data;
    }

    public void LoadFromData(ArmyEntityData data)
    {
        // so like if I want entities to be different from each other, 
        // I can save & load things here
        // ex. giving bonuses for being mvp in previous battles
    }
}

[System.Serializable]
public struct ArmyEntityData
{
    public string Id;
}

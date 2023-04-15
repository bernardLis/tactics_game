using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Ability")]
public class Ability : BaseScriptableObject
{
    [Header("Base Characteristics")]
    public string Description = "New Description";
    public Sprite Icon;
    public int BasePower;
    public int ManaCost;
    public Element Element;

    [Header("Battle GameObjects")]
    public GameObject AbilityExecutorPrefab;


    [Header("Other")]
    public int SpiceCost = 1;
    public int StarRank = 0;

    public AbilityData SerializeSelf()
    {
        AbilityData data = new();
        if (this == null)
            return data;

        data.TemplateId = Id;

        data.Name = name;
        return data;
    }
}

[Serializable]
public struct AbilityData
{
    public string TemplateId;
    public string Name;
    public int TimeLeftToCrafted;
}

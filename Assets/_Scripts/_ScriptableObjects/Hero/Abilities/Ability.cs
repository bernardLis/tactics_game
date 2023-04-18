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
    public int Level;
    public int ManaCost;
    public int BasePower;
    public int BaseCooldown;
    public int BaseScale;

    public Element Element;

    [Header("Battle GameObjects")]
    public GameObject AbilityExecutorPrefab;

    public event Action OnCooldownStarted;
    public void StartCooldown()
    {
        OnCooldownStarted?.Invoke();
    }

    public void LoadFromData(AbilityData data)
    {
        Level = data.Level;
    }

    public AbilityData SerializeSelf()
    {
        AbilityData data = new();
        if (this == null)
            return data;

        data.TemplateId = Id;

        data.Level = Level;
        return data;
    }
}

[Serializable]
public struct AbilityData
{
    public string TemplateId;
    public string Name;
    public int Level;
}

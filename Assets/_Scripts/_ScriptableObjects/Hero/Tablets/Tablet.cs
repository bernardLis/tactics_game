using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Tablet")]
public class Tablet : BaseScriptableObject
{
    public Sprite Icon;
    public string Description;
    public Element Element;

    [HideInInspector] public IntVariable Level;
    public int MaxLevel = 7;

    public StatType PrimaryStat;
    public StatType SecondaryStat;

    protected Hero _hero;

    public event Action OnLevelUp;
    public void Initialize(Hero hero)
    {
        Level = CreateInstance<IntVariable>();
        Level.SetValue(0);

        _hero = hero;
    }

    public virtual void LevelUp()
    {
        Debug.Log($"tablet {name} level up to {Level.Value + 1}");
        Level.ApplyChange(1);
        _hero.GetStatByType(PrimaryStat).LevelUp();
        if (SecondaryStat != StatType.None)
            _hero.GetStatByType(SecondaryStat).LevelUp();

        OnLevelUp?.Invoke();
    }

    public bool IsMaxLevel()
    {
        return Level.Value >= MaxLevel;
    }
}

using System;
using Lis.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Hero.Tablets
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Tablet")]
    public class Tablet : BaseScriptableObject
    {
        public Sprite Icon;
        public string Description;
        [FormerlySerializedAs("Element")] public Nature Nature;

        [HideInInspector] public IntVariable Level;
        public int MaxLevel = 7;
        public int Price;

        public StatLevelUpValue PrimaryStat;
        public StatLevelUpValue SecondaryStat;

        Hero _hero;

        public event Action<Tablet> OnLevelUp;

        public void Initialize(Hero hero)
        {
            Level = CreateInstance<IntVariable>();
            Level.SetValue(0);

            _hero = hero;
        }

        public void LevelUp()
        {
            if (IsMaxLevel()) return;

            Level.ApplyChange(1);

            if (PrimaryStat.StatType != StatType.None)
                _hero.GetStatByType(PrimaryStat.StatType).ApplyBaseValueChange(PrimaryStat.Value);
            if (SecondaryStat.StatType != StatType.None)
                _hero.GetStatByType(SecondaryStat.StatType).ApplyBaseValueChange(SecondaryStat.Value);

            OnLevelUp?.Invoke(this);
        }

        public bool IsMaxLevel()
        {
            return Level.Value >= MaxLevel;
        }
    }

    [Serializable]
    public struct StatLevelUpValue
    {
        public StatType StatType;
        public int Value;
    }
}
using System;


using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Tablet")]
    public class Tablet : BaseScriptableObject
    {
        public Sprite Icon;
        public string Description;
        public Element Element;

        [HideInInspector] public IntVariable Level;
        public int MaxLevel = 7;

        public StatLevelUpValue PrimaryStat;
        public StatLevelUpValue SecondaryStat;

        protected Hero _hero;

        public event Action OnLevelUp;
        public virtual void Initialize(Hero hero)
        {
            Level = CreateInstance<IntVariable>();
            Level.SetValue(0);

            _hero = hero;
        }

        public virtual void LevelUp()
        {
            if (IsMaxLevel()) return;
        
            Level.ApplyChange(1);

            if (PrimaryStat.StatType != StatType.None)
                _hero.GetStatByType(PrimaryStat.StatType).ApplyBaseValueChange(PrimaryStat.Value);
            if (SecondaryStat.StatType != StatType.None)
                _hero.GetStatByType(SecondaryStat.StatType).ApplyBaseValueChange(SecondaryStat.Value);

            OnLevelUp?.Invoke();
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
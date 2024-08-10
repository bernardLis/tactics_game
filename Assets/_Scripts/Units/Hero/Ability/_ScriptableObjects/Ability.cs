using System;
using System.Collections.Generic;
using Lis.Arena;
using Lis.Core;
using Lis.Units.Attack;
using Lis.Units.Hero.Tablets;
using Lis.Upgrades;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Hero.Ability
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Hero/Ability")]
    public class Ability : BaseScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperComment = "";
#endif

        [Header("Base Characteristics")]
        public string Description = "New Description";

        public Sprite Icon;
        public int Level;

        public List<AttackHeroAbility> Levels;

        [FormerlySerializedAs("Element")] public Nature Nature;
        [HideInInspector] public int KillCount;

        public bool IsAdvanced;

        [Header("Arena GameObjects")]
        public GameObject AbilityManagerPrefab;

        [Header("Sounds")]
        public Sound ExecuteSound;

        public int FightTimeActivated;

        float _cooldownMultiplier = 1f;

        Hero _hero;
        float _scaleMultiplier = 1f;

        public event Action OnCooldownStarted;
        public event Action OnLevelUp;

        public void InitializeFight(Hero hero)
        {
            _hero = hero;

            UpgradeBoard globalUpgradeBoard = GameManager.Instance.UpgradeBoard;
            float cooldownUpgrade = globalUpgradeBoard.GetUpgradeByName("Ability Cooldown").GetValue();
            float scaleUpgrade = globalUpgradeBoard.GetUpgradeByName("Ability Scale").GetValue();

            _cooldownMultiplier = 1 - cooldownUpgrade * 0.01f;
            _scaleMultiplier = 1 + scaleUpgrade * 0.01f;

            FightTimeActivated = Mathf.FloorToInt(FightManager.Instance.GetTime());

            Levels.ForEach(l => l.InitializeAttack(hero));
        }

        public float GetTotalDamageDealt()
        {
            int dmg = 0;
            foreach (Attack.Attack a in Levels)
                dmg += a.DamageDealt;
            return dmg;
        }

        public float GetDpsSinceActive()
        {
            return GetTotalDamageDealt() / (FightManager.Instance.GetTime() - FightTimeActivated);
        }

        public AttackHeroAbility GetCurrentLevel()
        {
            return Levels[Level];
        }

        public void StartCooldown()
        {
            OnCooldownStarted?.Invoke();
        }

        public int GetPower()
        {
            return GetPower(Level);
        }

        public int GetPower(int abilityLevel)
        {
            float pow = Levels[abilityLevel].Damage;
            if (_hero == null) return Mathf.FloorToInt(pow);
            pow += _hero.Power.GetValue(); // hero power
            Tablet t = _hero.GetTabletByElement(Nature.NatureName);
            if (t != null) pow += pow * t.Level.Value * 0.1f; // elemental bonus - tablet level * 10%

            return Mathf.FloorToInt(pow);
        }

        public float GetCooldown()
        {
            return GetCooldown(Level);
        }

        public float GetCooldown(int abilityLevel)
        {
            return Levels[abilityLevel].Cooldown * _cooldownMultiplier;
        }

        public float GetScale()
        {
            return GetScale(Level);
        }

        public float GetScale(int abilityLevel)
        {
            return Levels[abilityLevel].Scale * _scaleMultiplier;
        }

        public int GetAmount()
        {
            return GetAmount(Level);
        }

        public int GetAmount(int abilityLevel)
        {
            return Levels[abilityLevel].Amount;
        }

        public float GetDuration()
        {
            return GetDuration(Level);
        }

        public float GetDuration(int abilityLevel)
        {
            return Levels[abilityLevel].Duration;
        }

        public int GetPrice(int abilityLevel)
        {
            return Levels[abilityLevel].Price;
        }

        public bool IsMaxLevel()
        {
            return Level >= Levels.Count - 1;
        }

        public void AddKill()
        {
            KillCount++;
        }

        public void LevelUp()
        {
            if (Level >= Levels.Count - 1) return;

            Level++;
            OnLevelUp?.Invoke();
        }

        public void LevelDown()
        {
            if (Level > 0)
                Level--;
        }

        public void LoadFromData(AbilityData data)
        {
            Level = data.Level;
            KillCount = data.KillCount;
        }

        public AbilityData SerializeSelf()
        {
            AbilityData data = new();
            if (this == null)
                return data;

            data.TemplateId = Id;
            data.Level = Level;
            data.KillCount = KillCount;
            return data;
        }
    }

    [Serializable]
    public struct AbilityData
    {
        public string TemplateId;
        public string Name;
        public int Level;
        public int KillCount;
    }
}
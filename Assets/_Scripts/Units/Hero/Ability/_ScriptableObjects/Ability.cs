using System;
using System.Collections.Generic;
using Lis.Battle;
using Lis.Core;
using Lis.Units.Hero.Tablets;
using Lis.Upgrades;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Hero.Ability
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Ability")]
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

        public List<Level> Levels;
        public bool IsArmorPiercing;

        [FormerlySerializedAs("Element")] public Nature Nature;
        [HideInInspector] public int KillCount;

        public bool IsAdvanced;

        [Header("Battle GameObjects")]
        public GameObject AbilityManagerPrefab;

        [Header("Sounds")]
        public Sound ExecuteSound;

        public event Action OnCooldownStarted;
        public event Action OnLevelUp;

        public event Action OnStart;
        public event Action OnStop;

        float _cooldownMultiplier = 1f;
        float _scaleMultiplier = 1f;

        public int BattleTimeActivated;
        public int DamageDealt;

        Hero _hero;

        public void InitializeBattle(Hero hero)
        {
            _hero = hero;

            UpgradeBoard globalUpgradeBoard = GameManager.Instance.UpgradeBoard;
            float cooldownUpgrade = globalUpgradeBoard.GetUpgradeByName("Ability Cooldown").GetValue();
            float scaleUpgrade = globalUpgradeBoard.GetUpgradeByName("Ability Scale").GetValue();

            _cooldownMultiplier = 1 - cooldownUpgrade * 0.01f;
            _scaleMultiplier = 1 + scaleUpgrade * 0.01f;

            BattleTimeActivated = Mathf.FloorToInt(BattleManager.Instance.GetTime());
        }

        public void AddDamageDealt(int dmg)
        {
            DamageDealt += dmg;
        }

        public float GetDpsSinceActive()
        {
            return DamageDealt / (BattleManager.Instance.GetTime() - BattleTimeActivated);
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
            float pow = Levels[abilityLevel].Power;
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

        public void StartAbility()
        {
            OnStart?.Invoke();
        }

        public void StopAbility()
        {
            OnStop?.Invoke();
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
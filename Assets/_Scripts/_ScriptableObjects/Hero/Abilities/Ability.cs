using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
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

        public List<AbilityLevel> Levels;

        public Element Element;
        [HideInInspector] public int KillCount;

        public bool IsAdvanced;

        [Header("Battle GameObjects")]
        public GameObject AbilityManagerPrefab;

        public event Action OnCooldownStarted;
        public event Action OnLevelUp;

        public event Action OnStart;
        public event Action OnStop;

        float _cooldownMultiplier = 1f;
        float _scaleMultiplier = 1f;

        Hero _hero;

        public void InitializeBattle(Hero hero)
        {
            _hero = hero;

            UpgradeBoard globalUpgradeBoard = GameManager.Instance.UpgradeBoard;
            float cooldownUpgrade = globalUpgradeBoard.GetUpgradeByName("Ability Cooldown").GetValue();
            float scaleUpgrade = globalUpgradeBoard.GetUpgradeByName("Ability Scale").GetValue();

            _cooldownMultiplier = 1 - cooldownUpgrade * 0.01f;
            _scaleMultiplier = 1 + scaleUpgrade * 0.01f;
        }

        public void StartCooldown()
        {
            OnCooldownStarted?.Invoke();
        }

        public int GetPower()
        {
            float pow = Levels[Level].Power;
            if (_hero == null) return Mathf.FloorToInt(pow);
            pow += _hero.Power.GetValue(); // hero power
            Tablet t = _hero.GetTabletByElement(Element);
            if (t != null) pow += pow * t.Level.Value * 0.1f; // elemental bonus - tablet level * 10%


            return Mathf.FloorToInt(pow);
        }

        public float GetCooldown()
        {
            return Levels[Level].Cooldown * _cooldownMultiplier;
        }

        public float GetScale()
        {
            return Levels[Level].Scale * _scaleMultiplier;
        }

        public int GetAmount()
        {
            return Levels[Level].Amount;
        }

        public float GetDuration()
        {
            return Levels[Level].Duration;
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
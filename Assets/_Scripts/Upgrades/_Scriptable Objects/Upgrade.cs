using System;
using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Upgrades
{
    [CreateAssetMenu(menuName = "ScriptableObject/Upgrades/Upgrade")]
    public class Upgrade : BaseScriptableObject
    {
        GameManager _gameManager;

        public Sprite Icon;
        public string Description;
        public UpgradeType Type;

        public List<UpgradeLevel> Levels;

        [Tooltip("0 is unlocked, -1 is locked")]
        public int CurrentLevel = -1;

        public bool PermanentlyUnlocked;

        UpgradeBoard _board;

        public event Action OnLevelChanged;

        public void Initialize(UpgradeBoard board)
        {
            _gameManager = GameManager.Instance;
            _board = board;

            _board.OnRefundAll -= Refund;
            _board.OnUnlockAll -= PurchaseAll;

            _board.OnRefundAll += Refund;
            _board.OnUnlockAll += PurchaseAll;

            if (CurrentLevel > 0) return;
            if (PermanentlyUnlocked) CurrentLevel = 0;
        }

        void PurchaseAll()
        {
            while (!IsMaxLevel())
                Purchased();
        }

        public void Purchased()
        {
            CurrentLevel++;
            OnLevelChanged?.Invoke();
            _gameManager.SaveJsonData();
        }

        public UpgradeLevel GetCurrentLevel()
        {
            return Levels[CurrentLevel];
        }

        public UpgradeLevel GetNextLevel()
        {
            if (IsMaxLevel()) return null;

            return Levels[CurrentLevel + 1];
        }

        public bool IsMaxLevel()
        {
            return CurrentLevel == Levels.Count - 1;
        }

        public int GetValue()
        {
            int val = 0;
            for (int i = 0; i < Levels.Count; i++)
                if (i <= CurrentLevel)
                    val += Levels[i].Value;

            return val;
        }

        void Refund()
        {
            int val = 0;
            for (int i = 0; i < Levels.Count; i++)
                if (i <= CurrentLevel)
                    val += Levels[i].Cost;
            GameManager.Instance.ChangeGoldValue(val);
            if (PermanentlyUnlocked) CurrentLevel = 0;
            else CurrentLevel = -1;
            OnLevelChanged?.Invoke();
        }


        public UpgradeData SerializeSelf()
        {
            UpgradeData data = new()
            {
                Name = name,
                Level = CurrentLevel
            };

            return data;
        }

        public void LoadFromData(UpgradeData data)
        {
            CurrentLevel = data.Level;
        }
    }

    [Serializable]
    public struct UpgradeData
    {
        public string Name;
        public int Level;
    }

    public enum UpgradeType
    {
        Other,
        Hero,
        Building,
        Creature,
        Boss,
        Ability,
        Troops,
    }
}
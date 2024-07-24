using System;
using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Barracks/Nature Upgrade")]
    public class BarracksNatureUpgrade : BaseScriptableObject
    {
        public Nature Nature;
        public List<BarracksNatureUpgradeLevel> Levels;
        public int CurrentLevel;
        public string Description;

        public bool IsTokenActive;

        public event Action<Nature, int> OnUpgrade;

        public int GetUpgradePrice()
        {
            if (IsTokenActive && CurrentLevel == 0) return 0;

            return Levels[CurrentLevel + 1].Price;
        }

        public void Upgrade()
        {
            if (IsTokenActive) IsTokenActive = false;
            CurrentLevel++;
            OnUpgrade?.Invoke(Nature, CurrentLevel);
        }

        public bool IsMaxLevel()
        {
            return CurrentLevel == Levels.Count - 1;
        }
    }

    [Serializable]
    public struct BarracksNatureUpgradeLevel
    {
        public int Level;
        public int Price;
    }
}
using System;
using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Battle.Arena
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Barracks/Nature Upgrade")]
    public class BarracksNatureUpgrade : BaseScriptableObject
    {
        public Nature Nature;
        public List<BarracksNatureUpgradeLevel> Levels;
        public int CurrentLevel;
        public string Description;

        public int GetUpgradePrice()
        {
            return Levels[CurrentLevel + 1].Price;
        }

        public void Upgrade()
        {
            CurrentLevel++;
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
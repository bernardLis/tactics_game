using System;
using System.Collections.Generic;
using Lis.HeroCreation;
using Lis.Units.Hero;
using Lis.Upgrades;
using UnityEngine;

namespace Lis.Core.Utilities
{
    // https://www.youtube.com/watch?v=uD7y4T4PVk0\
// C:\Users\blis\AppData\LocalLow\DefaultCompany\Tactics Bu
    [Serializable]
    public class SaveData
    {
        // global data
        public int Seed;
        public int Gold;

        public List<VisualHeroData> VisualHeroes;
        public HeroData PlayerHero;

        public UpgradeBoardData GlobalUpgradeBoard;

        public GameStats GameStats;

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void LoadFromJson(string jsonString)
        {
            JsonUtility.FromJsonOverwrite(jsonString, this);
        }
    }

    public interface ISavable
    {
        void PopulateSaveData(SaveData saveData);
        void LoadFromSaveData(SaveData saveData);
    }
}
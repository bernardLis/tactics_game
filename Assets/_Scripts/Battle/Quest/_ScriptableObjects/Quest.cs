using System;
using System.Collections.Generic;
using Lis.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Battle.Quest
{
    public class Quest : BaseScriptableObject
    {
        public int Difficulty;
        public int TotalAmount;
        public int CurrentAmount;

        public event Action OnQuestUpdated;
        public event Action OnQuestCompleted;

        public virtual void CreateRandom(int level, List<Quest> previousQuests)
        {
            Difficulty = level;
            CurrentAmount = 0;
            TotalAmount = Random.Range(Difficulty, Difficulty + 20) + Difficulty;
        }

        public virtual void StartQuest()
        {
        }

        protected void UpdateQuest()
        {
            CurrentAmount++;
            OnQuestUpdated?.Invoke();

            if (CurrentAmount >= TotalAmount)
                OnQuestCompleted?.Invoke();
        }

        public virtual Sprite GetIcon()
        {
            // meant to be overwritten
            return null;
        }

        public virtual string GetDescription()
        {
            // meant to be overwritten
            return $"{name}";
        }
    }
}
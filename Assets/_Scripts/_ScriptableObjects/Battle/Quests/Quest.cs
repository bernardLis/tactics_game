using System;
using Random = UnityEngine.Random;

namespace Lis
{
    public class Quest : BaseScriptableObject
    {
        public int Difficulty;
        public int TotalAmount;
        public int CurrentAmount;

        public event Action OnQuestUpdated;
        public event Action OnQuestCompleted;

        public virtual void CreateRandom(int level)
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
    }
}
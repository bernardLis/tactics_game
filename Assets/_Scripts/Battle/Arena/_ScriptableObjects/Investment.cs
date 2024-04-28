using System;
using Lis.Battle.Fight;
using Lis.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Battle.Arena
{
    public class Investment : BaseScriptableObject
    {
        public int GoldToInvest;
        public float Interest;
        public int FightCount;
        public int GoldToReturn;

        public bool IsActive;
        public int FightsRemaining;

        public event Action OnStarted;
        public event Action OnCollected;
        public void CreateRandom()
        {
            GoldToInvest = Random.Range(100, 200);
            Interest = Random.Range(0.1f, 0.2f);
            FightCount = Random.Range(1, 5);
            // compound interest
            GoldToReturn = Mathf.RoundToInt(GoldToInvest * Mathf.Pow(1f + Interest, FightCount));
        }

        public void StartInvestment()
        {
            GameManager.Instance.ChangeGoldValue(-GoldToInvest);

            IsActive = true;
            FightManager fightManager = BattleManager.Instance.GetComponent<FightManager>();
            fightManager.OnFightEnded += OnFightEnded;
            FightsRemaining = FightCount;
            OnStarted?.Invoke();
        }

        void OnFightEnded()
        {
            FightsRemaining--;
        }

        public void Collect()
        {
            GameManager.Instance.ChangeGoldValue(GoldToReturn);
            IsActive = false;
            OnCollected?.Invoke();
        }
    }
}
using System;
using Lis.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Camp.Building
{
    public class Investment : BaseScriptableObject
    {
        public int GoldToInvest;
        public float Interest;
        public int NodeCount;
        public int GoldToReturn;

        public bool IsActive;
        public int NodesRemaining;

        public event Action OnStarted;
        public event Action OnCollected;

        public void CreateRandom()
        {
            GoldToInvest = Random.Range(100, 200);
            Interest = Random.Range(0.1f, 0.2f);
            NodeCount = Random.Range(1, 5);
            // compound interest
            GoldToReturn = Mathf.RoundToInt(GoldToInvest * Mathf.Pow(1f + Interest, NodeCount));
        }

        public void StartInvestment()
        {
            GameManager.Instance.ChangeGoldValue(-GoldToInvest);

            IsActive = true;
            NodesRemaining = NodeCount;
            OnStarted?.Invoke();
        }

        public void NodeCompleted()
        {
            NodesRemaining--;
        }

        public void Collect()
        {
            GameManager.Instance.ChangeGoldValue(GoldToReturn);
            IsActive = false;
            OnCollected?.Invoke();
        }
    }
}
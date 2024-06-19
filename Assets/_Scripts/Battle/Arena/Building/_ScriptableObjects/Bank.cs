using System.Collections.Generic;
using Lis.Battle.Fight;
using UnityEngine;

namespace Lis.Battle.Arena.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Bank")]
    public class Bank : Building
    {
        public List<Investment> AvailableInvestments = new();
        public List<Investment> ActiveInvestments = new();

        public override void Initialize(Battle battle)
        {
            base.Initialize(battle);
            AvailableInvestments = new();
            ActiveInvestments = new();
            CreateInvestments();
        }

        public void InitializeBattle()
        {
            BattleManager.Instance.GetComponent<FightManager>().OnFightEnded += CreateInvestments;
        }

        void CreateInvestments()
        {
            AvailableInvestments.Clear();
            for (int i = 0; i < 3; i++)
            {
                Investment investment = CreateInstance<Investment>();
                investment.CreateRandom();
                investment.OnStarted += () =>
                {
                    AvailableInvestments.Remove(investment);
                    ActiveInvestments.Add(investment);
                };
                investment.OnCollected += () => ActiveInvestments.Remove(investment);
                AvailableInvestments.Add(investment);
            }
        }
    }
}
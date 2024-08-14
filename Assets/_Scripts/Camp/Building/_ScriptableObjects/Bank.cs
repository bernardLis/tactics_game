using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Bank")]
    public class Bank : Building
    {
        [HideInInspector] public List<Investment> AvailableInvestments = new();
        [HideInInspector] public List<Investment> ActiveInvestments = new();

        public override void Initialize(Campaign campaign)
        {
            base.Initialize(campaign);
            AvailableInvestments = new();
            ActiveInvestments = new();
            CreateInvestments(3);
        }

        protected override void NodeCompleted()
        {
            base.NodeCompleted();
            if (!IsUnlocked) return;
            foreach (Investment i in ActiveInvestments)
                i.NodeCompleted();

            if (AvailableInvestments.Count < 5)
                CreateInvestments(2);
        }

        void CreateInvestments(int count)
        {
            for (int i = 0; i < count; i++)
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
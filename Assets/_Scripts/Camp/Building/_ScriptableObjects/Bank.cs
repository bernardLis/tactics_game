using System.Collections.Generic;
using Lis.Arena.Fight;
using Lis.Core;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Bank")]
    public class Bank : Building
    {
        public List<Investment> AvailableInvestments = new();
        public List<Investment> ActiveInvestments = new();

        public override void Initialize(Campaign campaign)
        {
            base.Initialize(campaign);
            AvailableInvestments = new();
            ActiveInvestments = new();
            CreateInvestments();
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
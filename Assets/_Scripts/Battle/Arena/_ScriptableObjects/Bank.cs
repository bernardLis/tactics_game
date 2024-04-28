using System.Collections.Generic;
using Lis.Battle.Fight;
using Lis.Core;
using UnityEngine;

namespace Lis.Battle.Arena
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Bank")]
    public class Bank : BaseScriptableObject
    {
        public List<Investment> AvailableInvestments = new();
        public List<Investment> ActiveInvestments = new();

        public void Initialize()
        {
            AvailableInvestments = new();
            ActiveInvestments = new();
            CreateInvestments();

            BattleManager.Instance.GetComponent<FightManager>().OnFightEnded += CreateInvestments;
        }

        public void CreateInvestments()
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
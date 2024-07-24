using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Boss;
using UnityEngine;

namespace Lis.Arena
{
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Arena")]
    public class Arena : BaseScriptableObject
    {
        public GameObject Prefab;

        [HideInInspector] public Boss Boss;
        [HideInInspector] public List<Fight.Fight> Fights = new();

        public void Initialize()
        {
            Boss = Instantiate(GameManager.Instance.UnitDatabase.GetRandomBoss());
            Boss.InitializeFight(1);

            Fights = new();
        }

        public Fight.Fight CreateFight(int heroPoints)
        {
            // TODO: balance
            const float exponent = 2.5f;
            const float multiplier = 2f;

            int result = Mathf.FloorToInt(multiplier * Mathf.Pow(Fights.Count, exponent));
            result = Mathf.RoundToInt(result * 0.1f) * 10; // rounding to tens
            int points = result + heroPoints;

            Fight.Fight fight = CreateInstance<Fight.Fight>();
            fight.Initialize(points, Fights.Count);
            Fights.Add(fight);

            return fight;
        }

        public int GetCurrentFightDifficulty()
        {
            // TODO: fight difficulty to spawn better orbs later
            return 1;
        }
    }
}
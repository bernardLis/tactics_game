using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Boss;
using UnityEngine;

namespace Lis.Battle.Arena
{
    using Fight;


    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Arena")]
    public class Arena : BaseScriptableObject
    {
        public GameObject Prefab;

        [Tooltip("How big sphere can fit the area")]
        public int Size;

        public int EnemyPoints;
        public int EnemyPointsGrowth;

        [HideInInspector] public Boss Boss;
        [HideInInspector] public List<Fight> Fights = new();

        public void Initialize(int level)
        {
            Boss = Instantiate(GameManager.Instance.UnitDatabase.GetRandomBoss());
            Boss.InitializeBattle(1);

            Fights = new();
        }

        public Fight CreateFight()
        {
            int points = EnemyPoints + EnemyPointsGrowth * Fights.Count;
            Fight fight = CreateInstance<Fight>();
            fight.Initialize(points);
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
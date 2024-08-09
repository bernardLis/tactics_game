using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Boss;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena
{
    using Fight;
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Arena")]
    public class Arena : BaseScriptableObject
    {
        public GameObject Prefab;

        [HideInInspector] public List<Wave> Waves;
        [HideInInspector] public Boss Boss;
        public int GoldReward;

        public Wave Initialize(Hero hero)
        {
            Boss = Instantiate(GameManager.Instance.UnitDatabase.GetRandomBoss());
            Boss.InitializeFight(1);

            Waves = new();
            return CreateFight(hero);
        }

        Wave CreateFight(Hero hero)
        {
            // TODO: balance
            int heroPoints = hero.GetHeroPoints();
            GoldReward = heroPoints * 10;

            int numberOfWaves = 3;
            for (int i = 0; i < numberOfWaves; i++)
            {
                const float exponent = 2.5f;
                const float multiplier = 2f;

                int result = Mathf.FloorToInt(multiplier * Mathf.Pow(numberOfWaves, exponent));
                result = Mathf.RoundToInt(result * 0.1f) * 10; // rounding to tens
                int points = result + heroPoints;

                Wave wave = CreateInstance<Wave>();
                wave.Initialize(points, hero);
                Waves.Add(wave);
            }

            return Waves[0];
        }
    }
}
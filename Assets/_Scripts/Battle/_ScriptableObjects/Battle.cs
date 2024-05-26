using System.Collections.Generic;
using Lis.Battle.Arena;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Battle")]
    public class Battle : BaseScriptableObject
    {
        public Stats Stats;

        public Hero SelectedHero;

        public List<Arena.Arena> Arenas;
        public Arena.Arena CurrentArena;

        public Bank Bank;
        public Building FightSelector;
        public Building FightStarter;
        public Building HeroLeveler;
        public Barracks Barracks;
        public Building RewardCollector;
        public Shop Shop;

        public void Initialize(int level)
        {
            Stats = CreateInstance<Stats>();

            foreach (Arena.Arena arena in Arenas)
                arena.Initialize(level);

            SetCurrentArena();
            SetRandomHero();
            InstantiateBuildings();
        }

        void SetCurrentArena()
        {
            CurrentArena = Arenas[0];
        }

        void SetRandomHero()
        {
            SelectedHero = Instantiate(GameManager.Instance.UnitDatabase.GetRandomHero());
            SelectedHero.InitializeHero();
        }

        void InstantiateBuildings()
        {
            Bank = Instantiate(Bank);
            Bank.Initialize();

            FightSelector = Instantiate(FightSelector);
            FightSelector.Initialize();

            FightStarter = Instantiate(FightStarter);
            FightStarter.Initialize();

            HeroLeveler = Instantiate(HeroLeveler);
            HeroLeveler.Initialize();

            Barracks = Instantiate(Barracks);
            Barracks.Initialize();

            RewardCollector = Instantiate(RewardCollector);
            RewardCollector.Initialize();

            Shop = Instantiate(Shop);
            Shop.Initialize();
        }
    }
}
using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle
{
    using Arena.Building;

    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Battle")]
    public class Battle : BaseScriptableObject
    {
        public Stats Stats;

        public Hero SelectedHero;

        public Arena.Arena CurrentArena;

        [Header("Buildings")]
        public Bank Bank;

        public Building FightSelector;
        public Building FightStarter;
        public Building HeroLeveler;
        public Barracks Barracks;
        public Building RewardCollector;
        public Shop Shop;
        public Architect Architect;

        public void Initialize(VisualHero visualHero)
        {
            Stats = CreateInstance<Stats>();

            ResolveHero(visualHero);
            InstantiateBuildings();
        }

        public void SetCurrentArena(Arena.Arena arena)
        {
            CurrentArena = arena;
        }

        void ResolveHero(VisualHero visualHero)
        {
            VisualHero vh = visualHero;
            if (vh == null)
            {
                vh = CreateInstance<VisualHero>();
                vh.Initialize();
                vh.RandomizeOutfit();
            }

            SelectedHero = Instantiate(vh.BodyType == 0
                ? GameManager.Instance.UnitDatabase.FemaleHero
                : GameManager.Instance.UnitDatabase.MaleHero);

            SelectedHero.InitializeHero(vh);
        }

        void InstantiateBuildings()
        {
            Bank = Instantiate(Bank);
            Bank.Initialize(this);

            FightSelector = Instantiate(FightSelector);
            FightSelector.Initialize(this);

            FightStarter = Instantiate(FightStarter);
            FightStarter.Initialize(this);
            FightStarter.IsUnlocked = true;

            HeroLeveler = Instantiate(HeroLeveler);
            HeroLeveler.Initialize(this);

            Barracks = Instantiate(Barracks);
            Barracks.Initialize(this);

            RewardCollector = Instantiate(RewardCollector);
            RewardCollector.Initialize(this);

            Shop = Instantiate(Shop);
            Shop.Initialize(this);

            Architect = Instantiate(Architect);
            Architect.Initialize(this);
        }

        public List<Building> GetAllBuildings()
        {
            return new List<Building>
            {
                Bank,
                FightSelector,
                FightStarter,
                HeroLeveler,
                Barracks,
                RewardCollector,
                Shop,
                Architect
            };
        }
    }
}
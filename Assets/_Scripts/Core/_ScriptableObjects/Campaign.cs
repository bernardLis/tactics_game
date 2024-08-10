using System.Collections.Generic;
using Lis.Camp.Building;
using Lis.Map.MapNodes;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Core
{
    using Map;
    using Arena;

    [CreateAssetMenu(menuName = "ScriptableObject/Campaign/Campaign")]
    public class Campaign : BaseScriptableObject
    {
        public Stats Stats;

        public Map Map;

        [HideInInspector] public Hero Hero;

        [HideInInspector] public MapNode CurrentHeroNode;
        [HideInInspector] public Arena CurrentArena;

        [Header("Buildings")]
        public Bank Bank;

        public Barracks Barracks;
        public Building RewardCollector;
        public Architect Architect;

        public void Initialize(VisualHero visualHero)
        {
            Stats = CreateInstance<Stats>();

            ResolveHero(visualHero);

            Map.Initialize();
            CurrentHeroNode = Map.MapRows[0].Nodes[0];

            InstantiateBuildings();
        }

        public void SetCurrentHeroNode(MapNode node)
        {
            CurrentHeroNode = node;
        }

        public void SetCurrentArena(Arena arena)
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

            Hero = Instantiate(vh.BodyType == 0
                ? GameManager.Instance.UnitDatabase.FemaleHero
                : GameManager.Instance.UnitDatabase.MaleHero);

            Hero.InitializeHero(vh);
        }

        void InstantiateBuildings()
        {
            Bank = Instantiate(Bank);
            Bank.Initialize(this);

            Barracks = Instantiate(Barracks);
            Barracks.Initialize(this);

            RewardCollector = Instantiate(RewardCollector);
            RewardCollector.Initialize(this);

            Architect = Instantiate(Architect);
            Architect.Initialize(this);
        }

        public List<Building> GetAllBuildings()
        {
            return new List<Building>
            {
                Bank,
                Barracks,
                RewardCollector,
                Architect
            };
        }
    }
}
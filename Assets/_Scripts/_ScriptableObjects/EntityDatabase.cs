using System.Collections.Generic;
using System.Linq;








using UnityEngine;

namespace Lis
{
    public class EntityDatabase : ScriptableObject
    {

        [Header("Creatures")]
        public GameObject OpponentProjectilePrefab;
        public Sprite[] CreatureIcons;

        public List<Creature> AllCreatures = new();
        public Creature GetRandomCreature() { return AllCreatures[Random.Range(0, AllCreatures.Count)]; }
        public Creature GetCreatureById(string id) { return AllCreatures.FirstOrDefault(x => x.Id == id); }
        public Creature GetRandomCreatureByUpgradeTier(int tier)
        {
            List<Creature> creatures = new();
            foreach (Creature c in AllCreatures)
                if (c.UpgradeTier == tier)
                    creatures.Add(c);

            return creatures[Random.Range(0, creatures.Count)];
        }
        public Creature GetRandomCreatureByUpgradeTierAndLower(int tier)
        {
            List<Creature> creatures = AllCreatures.Where(c => c.UpgradeTier <= tier).ToList();

            return creatures[Random.Range(0, creatures.Count)];
        }
        public List<Creature> GetCreaturesByTierElement(int tier, Element element)
        {
            List<Creature> creatures = new();
            foreach (Creature c in AllCreatures)
                if (c.UpgradeTier <= tier)
                    creatures.Add(c);

            for (int i = creatures.Count - 1; i >= 0; i--)
                if (creatures[i].Element != element)
                    creatures.RemoveAt(i);

            return creatures;
        }

        [Header("Minions")]
        [SerializeField] Minion[] Minions;
        public Creature RangedOpponent;

        public List<Minion> GetAllMinions() { return Minions.ToList(); }
        public List<Minion> GetAllMinionsByElement(Element element)
        {
            List<Minion> minions = new();
            foreach (Minion m in Minions)
                if (m.Element == element)
                    minions.Add(m);
            return minions;
        }

        public Minion GetRandomMinion() { return Minions[Random.Range(0, Minions.Length)]; }

        [SerializeField] StartingArmy[] StartingArmies;
        public StartingArmy GetStartingArmy(Element element) { return StartingArmies.FirstOrDefault(x => x.Element == element); }
        [System.Serializable]
        public struct StartingArmy
        {
            public Element Element;
            public List<Creature> Creatures;
        }
    
        [Header("Turrets")]
        [SerializeField] Turret[] Turrets;
        public List<Turret> GetAllTurrets() { return Turrets.ToList(); }
        public Turret GetRandomTurret() { return Turrets[Random.Range(0, Turrets.Length)]; }

        [Header("Abilities")]
        [SerializeField] Ability[] Abilities;
        public List<Ability> GetAllBasicAbilities() { return Abilities.ToList(); }
        public Ability GetAbilityById(string id) { return Abilities.FirstOrDefault(x => x.Id == id); }
        public Ability GetRandomAbility() { return Abilities[Random.Range(0, Abilities.Length)]; }

        [Header("Stats")]
        [SerializeField] StatBasics[] StatBasics;
        public Sprite GetStatIconByType(StatType type) { return StatBasics.FirstOrDefault(x => x.StatType == type).Sprite; }
        public string GetStatDescriptionByType(StatType type) { return StatBasics.FirstOrDefault(x => x.StatType == type).Description; }

        [SerializeField] Stat[] HeroStats;
        public Stat GetHeroStatByType(StatType type) { return HeroStats.FirstOrDefault(x => x.StatType == type); }
        public Tablet[] HeroTablets;
        [SerializeField] TabletAdvanced[] HeroTabletsAdvanced;
        public TabletAdvanced GetAdvancedTabletByElementNames(ElementName first, ElementName second)
        {
            foreach (TabletAdvanced t in HeroTabletsAdvanced)
                if (t.IsMadeOfElements(GetElementByName(first), GetElementByName(second)))
                    return t;
            return null;
        }


        [Header("Elements")]
        [SerializeField] Element[] Elements;
        public List<Element> GetAllElements() { return Elements.ToList(); }
        public Element GetRandomElement() { return Elements[Random.Range(0, Elements.Length)]; }
        Element GetElementByName(ElementName str) { return Elements.FirstOrDefault(x => x.ElementName == str); }
    }
}

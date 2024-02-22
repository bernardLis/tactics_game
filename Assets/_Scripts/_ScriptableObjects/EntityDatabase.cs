using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis
{
    public class EntityDatabase : ScriptableObject
    {
        public void Initialize()
        {
            foreach (Hero h in Heroes)
            {
                h.TimesPicked = PlayerPrefs.GetInt(h.Id, 0);
            }
        }

        [Header("Hero")]
        public Hero[] Heroes;

        public Tablet[] HeroTablets;

        [FormerlySerializedAs("HeroTabletsAdvanced")] [SerializeField]
        TabletAdvanced[] _heroTabletsAdvanced;

        public TabletAdvanced GetAdvancedTabletByElementNames(ElementName first, ElementName second)
        {
            foreach (TabletAdvanced t in _heroTabletsAdvanced)
                if (t.IsMadeOfElements(GetElementByName(first), GetElementByName(second)))
                    return t;
            return null;
        }

        [Header("Creatures")]
        public Sprite[] CreatureIcons;

        public List<Creature> AllCreatures = new();

        public Creature GetRandomCreature()
        {
            return AllCreatures[Random.Range(0, AllCreatures.Count)];
        }

        public Creature GetCreatureById(string id)
        {
            return AllCreatures.FirstOrDefault(x => x.Id == id);
        }

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

        [FormerlySerializedAs("Minions")] [Header("Minions")] [SerializeField]
        Minion[] _minions;

        [SerializeField] List<Entity> _rangedOpponents;

        public Entity GetRandomRangedOpponent()
        {
            return _rangedOpponents[Random.Range(0, _rangedOpponents.Count)];
        }

        public List<Minion> GetAllMinions()
        {
            return _minions.ToList();
        }

        public Minion GetRandomMinionByElement(ElementName elName)
        {
            List<Minion> minions = new();
            foreach (Minion m in _minions)
                if (m.Element.ElementName == elName)
                    minions.Add(m);
            return minions[Random.Range(0, minions.Count)];
        }

        public Minion GetRandomMinion()
        {
            return _minions[Random.Range(0, _minions.Length)];
        }

        [System.Serializable]
        public struct StartingArmy
        {
            public Element Element;
            public List<Creature> Creatures;
        }

        [FormerlySerializedAs("Turrets")] [Header("Turrets")] [SerializeField]
        Turret[] _turrets;

        public List<Turret> GetAllTurrets()
        {
            return _turrets.ToList();
        }

        public Turret GetRandomTurret()
        {
            return _turrets[Random.Range(0, _turrets.Length)];
        }

        [FormerlySerializedAs("Abilities")] [Header("Abilities")] [SerializeField]
        Ability[] _abilities;

        public List<Ability> GetAllBasicAbilities()
        {
            return _abilities.ToList();
        }

        public Ability GetAbilityById(string id)
        {
            return _abilities.FirstOrDefault(x => x.Id == id);
        }

        public Ability GetRandomAbility()
        {
            return _abilities[Random.Range(0, _abilities.Length)];
        }

        [FormerlySerializedAs("StatBasics")] [Header("Stats")] [SerializeField]
        StatBasics[] _statBasics;

        public Sprite GetStatIconByType(StatType type)
        {
            return _statBasics.FirstOrDefault(x => x.StatType == type).Sprite;
        }

        public string GetStatDescriptionByType(StatType type)
        {
            return _statBasics.FirstOrDefault(x => x.StatType == type).Description;
        }


        [FormerlySerializedAs("Elements")] [Header("Elements")] [SerializeField]
        Element[] _elements;

        public List<Element> GetAllElements()
        {
            return _elements.ToList();
        }

        public Element GetRandomElement()
        {
            return _elements[Random.Range(0, _elements.Length)];
        }

        Element GetElementByName(ElementName str)
        {
            return _elements.FirstOrDefault(x => x.ElementName == str);
        }
    }
}
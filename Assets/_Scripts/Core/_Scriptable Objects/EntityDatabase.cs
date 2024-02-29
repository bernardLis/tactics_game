using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Core
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

        [SerializeField] Ability[] _abilities;

        public List<Ability> GetAllBasicAbilities()
        {
            return _abilities.ToList();
        }

        public Ability GetAbilityById(string id)
        {
            return _abilities.FirstOrDefault(x => x.Id == id);
        }


        public Tablet[] HeroTablets;
        [SerializeField] TabletAdvanced[] _heroTabletsAdvanced;

        public TabletAdvanced GetAdvancedTabletByElementNames(ElementName first, ElementName second)
        {
            foreach (TabletAdvanced t in _heroTabletsAdvanced)
                if (t.IsMadeOfElements(GetElementByName(first), GetElementByName(second)))
                    return t;
            return null;
        }

        [Header("Minions")]
        [SerializeField] Minion[] _minions;

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

        [Header("Creatures")]
        public Sprite[] CreatureIcons;

        public List<Creature> AllCreatures = new();


        [Header("Bosses")]
        [SerializeField] Boss[] _bosses;

        public Boss GetRandomBoss() => _bosses[Random.Range(0, _bosses.Length)];


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


        [Header("Elements")] [SerializeField]
        Element[] _elements;

        Element GetElementByName(ElementName str)
        {
            return _elements.FirstOrDefault(x => x.ElementName == str);
        }
    }
}
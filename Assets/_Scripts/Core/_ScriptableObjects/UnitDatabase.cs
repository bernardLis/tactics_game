using System;
using System.Collections.Generic;
using System.Linq;
using Lis.Units;
using Lis.Units.Boss;
using Lis.Units.Creature;
using Lis.Units.Hero;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Tablets;
using Lis.Units.Minion;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis.Core
{
    public class UnitDatabase : ScriptableObject
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
        [SerializeField] Ability[] _advancedAbilities;


        public List<Ability> GetAllBasicAbilities()
        {
            return _abilities.ToList();
        }

        public List<Ability> GetAllAbilities()
        {
            return _abilities.Concat(_advancedAbilities).ToList();
        }

        public Ability GetAbilityById(string id)
        {
            return _abilities.FirstOrDefault(x => x.Id == id);
        }


        public Tablet[] HeroTablets;
        [SerializeField] TabletAdvanced[] _heroTabletsAdvanced;

        public List<TabletAdvanced> GetAllAdvancedTablets()
        {
            return _heroTabletsAdvanced.ToList();
        }

        public TabletAdvanced GetAdvancedTabletByElementNames(NatureName first, NatureName second)
        {
            foreach (TabletAdvanced t in _heroTabletsAdvanced)
                if (t.IsMadeOfElements(GetElementByName(first), GetElementByName(second)))
                    return t;
            return null;
        }

        [Header("Minions")]
        [SerializeField] Minion[] _minions;

        public List<Minion> GetAllMinions()
        {
            return _minions.ToList();
        }

        public Minion GetMinionByLevel(int level)
        {
            Debug.Log("GetMinionByLevel: " + level);
            List<Minion> minions = new();
            foreach (Minion m in _minions)
                if (m.LevelRange.x < level && m.LevelRange.y > level)
                    minions.Add(m);
            return minions[Random.Range(0, minions.Count)];
        }

        public Minion GetRandomMinion()
        {
            return _minions[Random.Range(0, _minions.Length)];
        }

        [SerializeField] List<Unit> _rangedOpponents;

        public Unit GetRandomRangedOpponent()
        {
            return _rangedOpponents[Random.Range(0, _rangedOpponents.Count)];
        }

        public Unit GetRangedOpponentByNature(NatureName natureName)
        {
            return _rangedOpponents.FirstOrDefault(x => x.Nature.NatureName == natureName);
        }

        [Serializable]
        public struct MinionMaterial
        {
            public NatureName NatureName;
            public Material Material;
        }

        public MinionMaterial[] MinionMaterials;

        public Material GetMinionMaterialByNature(NatureName natureName)
        {
            return MinionMaterials.FirstOrDefault(x => x.NatureName == natureName).Material;
        }


        [Header("Creatures")]
        public Sprite[] CreatureIcons;

        public List<Creature> AllCreatures = new();


        [Header("Bosses")]
        [SerializeField] Boss[] _bosses;

        public List<Boss> GetAllBosses()
        {
            return _bosses.ToList();
        }

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
        Nature[] _elements;

        Nature GetElementByName(NatureName str)
        {
            return _elements.FirstOrDefault(x => x.NatureName == str);
        }
    }
}
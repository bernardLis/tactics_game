using System.Collections.Generic;
using System.Linq;
using Lis.Units.Boss;
using Lis.Units.Creature;
using Lis.Units.Enemy;
using Lis.Units.Hero;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Tablets;
using Lis.Units.Pawn;
using Lis.Units.Peasant;
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
                h.TimesPicked = PlayerPrefs.GetInt(h.Id, 0);
        }

        [Header("Hero")]
        public Hero[] Heroes;

        public Hero GetRandomHero()
        {
            return Heroes[Random.Range(0, Heroes.Length)];
        }

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

        public TabletAdvanced GetAdvancedTabletByNatureNames(NatureName first, NatureName second)
        {
            foreach (TabletAdvanced t in _heroTabletsAdvanced)
                if (t.IsMadeOfNatures(GetNatureByName(first), GetNatureByName(second)))
                    return t;
            return null;
        }

        [Header("Peasant")]
        public Peasant Peasant;

        [Header("Pawns")]
        [SerializeField] Pawn[] _allPawns;

        public Pawn GetRandomPawn()
        {
            return _allPawns[Random.Range(0, _allPawns.Length)];
        }

        public Pawn GetPawnByNature(Nature n)
        {
            return _allPawns.FirstOrDefault(x => x.Nature == n);
        }

        [Header("Creatures")]
        public Sprite[] CreatureIcons;

        public List<Creature> AllCreatures = new();

        public Creature GetRandomCreature()
        {
            return AllCreatures[Random.Range(0, AllCreatures.Count)];
        }

        [SerializeField] Creature[] _opponentCreatures;

        public List<Creature> GetAllOpponentCreatures()
        {
            return _opponentCreatures.ToList();
        }

        public Creature GetRandomOpponentCreature()
        {
            return _opponentCreatures[Random.Range(0, _opponentCreatures.Length)];
        }

        [Header("Enemies")]
        [SerializeField] Enemy[] _enemies;

        public List<Enemy> GetAllEnemies()
        {
            return _enemies.ToList();
        }


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


        [Header("Natures")] [SerializeField]
        Nature[] _natures;

        public Nature GetNatureByName(NatureName str)
        {
            return _natures.FirstOrDefault(x => x.NatureName == str);
        }

        [SerializeField] NatureAdvanced[] _advancedNatures;

        public NatureAdvanced GetRandomAdvancedNature()
        {
            return _advancedNatures[Random.Range(0, _advancedNatures.Length)];
        }
    }
}
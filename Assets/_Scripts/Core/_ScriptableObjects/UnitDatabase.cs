using System.Collections.Generic;
using System.Linq;
using Lis.Units;
using Lis.Units.Boss;
using Lis.Units.Creature;
using Lis.Units.Enemy;
using Lis.Units.Hero;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Items;
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
        [Header("Hero")]
        [SerializeField] Ability[] _abilities;

        [SerializeField] Ability[] _advancedAbilities;


        public Tablet[] HeroTablets;
        [SerializeField] TabletAdvanced[] _heroTabletsAdvanced;

        /* FEMALE */
        public Hero FemaleHero;
        [SerializeField] Item[] _allFemaleHeroOutfits;

        public List<Item> GetAllFemaleHeroOutfits()
        {
            return _allFemaleHeroOutfits.ToList();
        }

        public Item GetFemaleOutfitById(string id)
        {
            return _allFemaleHeroOutfits.FirstOrDefault(x => x.Id == id);
        }

        readonly Dictionary<ItemType, List<Item>> _femaleOutfitDictionary = new();

        public Item GetRandomFemaleItemByType(ItemType type)
        {
            if (!_femaleOutfitDictionary.ContainsKey(type))
                _femaleOutfitDictionary[type] = _allFemaleHeroOutfits.Where(x => x.ItemType == type).ToList();

            return _femaleOutfitDictionary[type][Random.Range(0, _femaleOutfitDictionary[type].Count)];
        }

        [SerializeField] Item[] _allFemaleHeroArmor;
        public List<Item> GetAllFemaleHeroArmor => _allFemaleHeroArmor.ToList();

        public Armor GetRandomFemaleArmor()
        {
            return _allFemaleHeroArmor[Random.Range(0, _allFemaleHeroArmor.Length)] as Armor;
        }


        /* MALE */
        public Hero MaleHero;

        [SerializeField] Item[] _allMaleHeroOutfits;

        public List<Item> GetAllMaleHeroOutfits()
        {
            return _allMaleHeroOutfits.ToList();
        }

        public Item GetMaleOutfitById(string id)
        {
            return _allMaleHeroOutfits.FirstOrDefault(x => x.Id == id);
        }

        readonly Dictionary<ItemType, List<Item>> _maleOutfitDictionary = new();

        public Item GetRandomMaleItemByType(ItemType type)
        {
            if (!_maleOutfitDictionary.ContainsKey(type))
                _maleOutfitDictionary[type] = _allMaleHeroOutfits.Where(x => x.ItemType == type).ToList();

            return _maleOutfitDictionary[type][Random.Range(0, _maleOutfitDictionary[type].Count)];
        }

        [SerializeField] Item[] _allMaleHeroArmor;
        public List<Item> GetAllMaleHeroArmor => _allMaleHeroArmor.ToList();

        public Armor GetRandomMaleArmor()
        {
            return _allMaleHeroArmor[Random.Range(0, _allMaleHeroArmor.Length)] as Armor;
        }

        [SerializeField] Color[] _heroCustomizationColors;
        public List<Color> GetAllHeroCustomizationColors() => _heroCustomizationColors.ToList();

        public Color GetRandomHeroCustomizationColor() =>
            _heroCustomizationColors[Random.Range(0, _heroCustomizationColors.Length)];

        [Header("Peasant")]
        public Peasant Peasant;

        [Header("Pawns")]
        [SerializeField]
        Pawn[] _allPawns;

        [Header("Creatures")]
        public Sprite[] CreatureIcons;

        public List<Creature> AllCreatures = new();

        [SerializeField] Creature[] _opponentCreatures;

        [Header("Enemies")]
        [SerializeField]
        Enemy[] _enemies;

        [Header("Bosses")]
        [SerializeField]
        Boss[] _bosses;


        [FormerlySerializedAs("StatBasics")] [Header("Stats")] [SerializeField]
        StatBasics[] _statBasics;


        [Header("Natures")] [SerializeField] Nature[] _natures;

        [SerializeField] NatureAdvanced[] _advancedNatures;


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

        public List<Pawn> GetAllPawns()
        {
            return _allPawns.ToList();
        }

        public Pawn GetRandomPawn()
        {
            return _allPawns[Random.Range(0, _allPawns.Length)];
        }

        public Pawn GetPawnByNature(Nature n)
        {
            return _allPawns.FirstOrDefault(x => x.Nature == n);
        }

        public Creature GetRandomCreature()
        {
            return AllCreatures[Random.Range(0, AllCreatures.Count)];
        }

        public List<Creature> GetAllOpponentCreatures()
        {
            return _opponentCreatures.ToList();
        }

        public Creature GetRandomOpponentCreature()
        {
            return _opponentCreatures[Random.Range(0, _opponentCreatures.Length)];
        }

        public List<Enemy> GetAllEnemies()
        {
            return _enemies.ToList();
        }

        public List<Enemy> GetAllRangedEnemies()
        {
            return _enemies.Where(x => x.IsRanged).ToList();
        }

        public Enemy GetEnemyByName(string n)
        {
            return _enemies.FirstOrDefault(x => x.name == n);
        }

        public Unit GetUnitById(string id)
        {
            List<Unit> units = new(_enemies.ToList());
            units.AddRange(AllCreatures);
            units.AddRange(_allPawns);
            units.Add(Peasant);

            return units.FirstOrDefault(x => x.Id == id);
        }

        public List<Boss> GetAllBosses()
        {
            return _bosses.ToList();
        }

        public Boss GetRandomBoss()
        {
            return _bosses[Random.Range(0, _bosses.Length)];
        }

        public Sprite GetStatIconByType(StatType type)
        {
            return _statBasics.FirstOrDefault(x => x.StatType == type).Sprite;
        }

        public string GetStatDescriptionByType(StatType type)
        {
            return _statBasics.FirstOrDefault(x => x.StatType == type).Description;
        }

        public Nature GetNatureByName(NatureName str)
        {
            return _natures.FirstOrDefault(x => x.NatureName == str);
        }

        public NatureAdvanced GetRandomAdvancedNature()
        {
            return _advancedNatures[Random.Range(0, _advancedNatures.Length)];
        }

        public Nature GetRandomNature()
        {
            return _natures[Random.Range(0, _natures.Length)];
        }
    }
}
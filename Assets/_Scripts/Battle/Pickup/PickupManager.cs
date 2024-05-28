using System;
using System.Collections.Generic;
using Lis.Battle.Fight;
using Lis.Core.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Battle.Pickup
{
    public class PickupManager : PoolManager<PickupController>
    {
        FightManager _fightManager;

        [SerializeField] PickupController _pickupControllerPrefab;

        [SerializeField] Coin _coin;
        [SerializeField] Hammer _hammer;
        [SerializeField] Horseshoe _horseshoe;
        [SerializeField] Bag _bag;
        [SerializeField] Skull _skull;
        [SerializeField] Dice _dice;
        [SerializeField] Mushroom _mushroom;

        [SerializeField] List<ExperienceStone> _expOrbs = new();

        public event Action<Pickup> OnPickupCollected;

        public void Initialize()
        {
            CreatePool(_pickupControllerPrefab.gameObject);

            _fightManager = FightManager.Instance;
            _fightManager.OnFightEnded += OnFightEnded;
            // HERE: testing
            GetComponent<InputManager>().OnTwoClicked += SpawnBunchExpStones;
        }

        bool _firstFight;

        void OnFightEnded()
        {
            if (_firstFight) return;
            _firstFight = true;

            SpawnPickup(Instantiate(_mushroom), new(-23, 0, 0));
        }


        public void SpawnBunchExpStones()
        {
            for (int i = 0; i < 50; i++)
                SpawnExpStone(new(Random.Range(-10, 10), 10, Random.Range(-10, 10)));
        }

        public void SpawnExpStone(Vector3 position)
        {
            ExperienceStone stone = ChooseExpStone();
            if (stone == null) return;
            PickupController pickupController = GetObjectFromPool();
            pickupController.Initialize(stone, position);
            pickupController.OnCollected += PickupCollected;
        }

        ExperienceStone ChooseExpStone()
        {
            int v = Random.Range(0, 101);
            List<ExperienceStone> possibleOrbs = new();
            foreach (ExperienceStone orb in _expOrbs)
                if (v <= orb.OrbChance && 1 >= orb.MinDifficulty) // TODO: fight difficulty to spawn better orbs later
                    possibleOrbs.Add(orb);

            // return the exp orb with the lowest chance
            if (possibleOrbs.Count > 0)
            {
                ExperienceStone lowestChanceStone = possibleOrbs[0];
                foreach (ExperienceStone l in possibleOrbs)
                    if (l.OrbChance < lowestChanceStone.OrbChance)
                        lowestChanceStone = l;

                ExperienceStone instance = Instantiate(lowestChanceStone);
                return instance;
            }

            return null;
        }

        public void SpawnPickup(Vector3 position)
        {
            // 2% chance of spawning hammer
            // 2% chance of spawning horseshoe
            // ...
            // rest% chance of spawning coin
            int random = Random.Range(0, 100);
            if (random > 26) return; // chance there is no pickup

            Pickup p = Instantiate(_mushroom); //Instantiate(_coin);

            if (random == 0)
                p = Instantiate(_hammer);
            else if (random == 1)
                p = Instantiate(_horseshoe);
            else if (random == 2)
                p = Instantiate(_bag);
            // else if (random == 3)
            //     p = Instantiate(_skull);
            else if (random == 4)
                p = Instantiate(_dice);
            else if (random == 5)
                p = Instantiate(_mushroom);

            SpawnPickup(p, position);
        }

        void SpawnPickup(Pickup p, Vector3 pos)
        {
            PickupController pickupController = GetObjectFromPool();
            pickupController.Initialize(p, pos);
            pickupController.OnCollected += PickupCollected;
        }

        void PickupCollected(PickupController pickupController)
        {
            OnPickupCollected?.Invoke(pickupController.Pickup);
            pickupController.OnCollected -= PickupCollected;
        }

        public void BagCollected()
        {
            foreach (PickupController p in GetActiveObjects())
                if (p.Pickup is Coin)
                    p.GetToHero();
        }

        public void HorseshoeCollected()
        {
            foreach (PickupController p in GetActiveObjects())
                if (p.Pickup is ExperienceStone)
                    p.GetToHero();
        }
    }
}
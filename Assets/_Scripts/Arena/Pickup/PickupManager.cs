using System;
using System.Collections.Generic;
using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Arena.Pickup
{
    public class PickupManager : PoolManager<PickupController>
    {
        [SerializeField] PickupController _pickupControllerPrefab;

        [SerializeField] Coin _coin;
        [SerializeField] Hammer _hammer;
        [SerializeField] Horseshoe _horseshoe;
        [SerializeField] Bag _bag;
        [SerializeField] Skull _skull;
        [SerializeField] Dice _dice;
        [SerializeField] Mushroom _mushroom;
        [SerializeField] BarracksToken _barracksToken;
        [SerializeField] HealthPickup _healthPickup;

        [SerializeField] List<ExperienceStone> _expOrbs = new();

        public event Action<Pickup> OnPickupCollected;

        public void Initialize()
        {
            CreatePool(_pickupControllerPrefab.gameObject);
            // HERE: testing
            GetComponent<InputManager>().OnTwoClicked += SpawnBunchExpStones;
        }


        public void SpawnBunchExpStones()
        {
            for (int i = 0; i < 50; i++)
                SpawnExpStone(default, new(Random.Range(-10, 10), 10, Random.Range(-10, 10)));
        }

        public void SpawnExpStone(Unit unit, Vector3 position)
        {
            int scarinessRank = 0;
            if (unit is Enemy enemy) scarinessRank = enemy.ScarinessRank;
            ExperienceStone stone = ChooseExpStone(scarinessRank);
            if (stone == null) return;
            PickupController pickupController = GetObjectFromPool();
            Vector3 pos = position;
            pos.y = 5;
            pickupController.Initialize(stone, pos);
            pickupController.OnCollected += PickupCollected;
        }

        ExperienceStone ChooseExpStone(int rank)
        {
            int v = Random.Range(0, 101);
            List<ExperienceStone> possibleOrbs = new();
            foreach (ExperienceStone orb in _expOrbs)
                if (v <= orb.OrbChance && rank >= orb.MinScariness)
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
            // 1% chance of spawning hammer
            // 1% chance of spawning horseshoe
            // ...
            // rest% chance of spawning coin
            int random = Random.Range(0, 100);
            if (random > 26) return; // chance there is no pickup

            Pickup p = Instantiate(_coin);

            // if (random == 0)
            //     p = Instantiate(_hammer);
            // else if (random == 1)
            //     p = Instantiate(_horseshoe);
            // else if (random == 2)
            //     p = Instantiate(_bag);
            // else if (random == 3)
            //     p = Instantiate(_skull);
            if (random == 4)
                p = Instantiate(_dice);
            else if (random == 5)
                p = Instantiate(_mushroom);
            else if (random == 6 || random == 7)
                p = Instantiate(_healthPickup);


            SpawnPickup(p, position);
        }

        public void SpawnPickup(Pickup p, Vector3 pos)
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

        public PickupController GetPawnCollectablePickup()
        {
            List<PickupController> avPickups = new();
            foreach (PickupController p in GetActiveObjects())
                if (p.Pickup is Coin or ExperienceStone)
                    avPickups.Add(p);

            return avPickups.Count > 0 ? avPickups[Random.Range(0, avPickups.Count)] : null;
        }
    }
}
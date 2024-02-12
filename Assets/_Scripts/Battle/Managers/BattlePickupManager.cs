using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattlePickupManager : PoolManager<BattlePickup>
    {
        [SerializeField] BattlePickup _pickupPrefab;

        [SerializeField] Coin _coin;
        [SerializeField] Hammer _hammer;
        [SerializeField] Horseshoe _horseshoe;
        [SerializeField] Bag _bag;
        [SerializeField] Skull _skull;
        [SerializeField] FriendBall _friendBall;


        [FormerlySerializedAs("ExpOrbs")] [SerializeField]
        List<ExperienceOrb> _expOrbs = new();

        public event Action<Pickup> OnPickupCollected;

        public void Initialize()
        {
            CreatePool(_pickupPrefab.gameObject);
        }

        public void SpawnExpOrb(Vector3 position)
        {
            ExperienceOrb orb = ChooseExpOrb();
            if (orb == null) return;
            BattlePickup battlePickup = GetObjectFromPool();
            battlePickup.Initialize(orb, position);
            battlePickup.OnCollected += PickupCollected;
        }

        ExperienceOrb ChooseExpOrb()
        {
            int v = Random.Range(0, 101);
            List<ExperienceOrb> possibleOrbs = new();
            foreach (ExperienceOrb orb in _expOrbs)
                if (v <= orb.OrbChance)
                    possibleOrbs.Add(orb);

            // return the exp orb with the lowest chance
            if (possibleOrbs.Count > 0)
            {
                ExperienceOrb lowestChanceOrb = possibleOrbs[0];
                foreach (ExperienceOrb l in possibleOrbs)
                    if (l.OrbChance < lowestChanceOrb.OrbChance)
                        lowestChanceOrb = l;

                ExperienceOrb instance = Instantiate(lowestChanceOrb);
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
            Pickup p = Instantiate(_coin);

            if (random == 0 || random == 1)
                p = Instantiate(_hammer);
            else if (random == 2 || random == 3)
                p = Instantiate(_horseshoe);
            else if (random == 4 || random == 5)
                p = Instantiate(_bag);
            else if (random == 6 || random == 7)
                p = Instantiate(_skull);
            else if (random == 8 || random == 9)
                p = Instantiate(_friendBall);

            BattlePickup battlePickup = GetObjectFromPool();
            battlePickup.Initialize(p, position);

            battlePickup.OnCollected += PickupCollected;
        }

        void PickupCollected(BattlePickup battlePickup)
        {
            OnPickupCollected?.Invoke(battlePickup.Pickup);
            battlePickup.OnCollected -= PickupCollected;
        }

        public void BagCollected()
        {
            foreach (BattlePickup p in GetActiveObjects())
                if (p.Pickup is Coin)
                    p.GetToHero();
        }

        public void HorseshoeCollected()
        {
            foreach (BattlePickup p in GetActiveObjects())
                if (p.Pickup is ExperienceOrb)
                    p.GetToHero();
        }
    }
}
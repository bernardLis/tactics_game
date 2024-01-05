using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePickupManager : PoolManager<BattlePickup>
{
    [SerializeField] BattlePickup _pickupPrefab;

    [SerializeField] Coin _coin;
    [SerializeField] Hammer _hammer;
    [SerializeField] Horseshoe _horseshoe;
    [SerializeField] Bag _bag;
    [SerializeField] Skull _skull;


    public void Initialize()
    {
        CreatePool(_pickupPrefab.gameObject);
    }

    public void SpawnPickup(Vector3 position)
    {
        // 1% chance of spawning hammer
        // 1% chance of spawning horseshoe
        // 98% chance of spawning coin
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

        BattlePickup battlePickup = GetObjectFromPool();
        battlePickup.Initialize(p, position);
    }

    public void BagCollected()
    {
        foreach (BattlePickup p in GetActiveObjects())
            if (p.Pickup is Coin)
                p.GetToHero();
    }
}

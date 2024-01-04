using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePickupManager : PoolManager<BattlePickup>
{
    [SerializeField] BattlePickup _pickupPrefab;

    public void Initialize()
    {
        CreatePool(_pickupPrefab.gameObject);
    }

    public void SpawnPickup(Pickup pickup, Vector3 position)
    {
        BattlePickup p = GetObjectFromPool();
        p.Initialize(pickup, position);
    }

    public void BagCollected()
    {
        foreach (BattlePickup p in GetActiveObjects())
            if (p.Pickup is Coin)
                p.GetToHero();
    }
}

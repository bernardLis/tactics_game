using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLandPurchaseSign : MonoBehaviour
{
    public BattleLandTile _tileToPurchase;

    public event Action OnPurchased;
    public void Initialize(BattleLandTile tile)
    {
        Debug.Log($"_tileToPurchase {_tileToPurchase}");
        _tileToPurchase = tile;
    }

    public void OnTriggerEnter(Collider collider)
    {
        Debug.Log($"triggered {collider.name}");

        if (collider.TryGetComponent(out BattleHero hero))
        {
            PurchaseTile();
        }

    }

    void PurchaseTile()
    {
        Debug.Log($"purchase tile {_tileToPurchase}");
        _tileToPurchase.gameObject.SetActive(true);
        OnPurchased?.Invoke();

    }
}

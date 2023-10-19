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
        _tileToPurchase = tile;
        GetComponent<MeshRenderer>().material = tile.ChosenMaterial;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out BattleHero hero))
            PurchaseTile();
    }

    void PurchaseTile()
    {
        _tileToPurchase.EnableTile();
        OnPurchased?.Invoke();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleTilePurchaseSign : MonoBehaviour
{
    public BattleTile _tileToPurchase;

    GameObject _tileIndicator;

    public event Action OnPurchased;
    public void Initialize(BattleTile tile)
    {
        _tileToPurchase = tile;

        _tileIndicator = Instantiate(_tileToPurchase.TileIndicationPrefab, transform);
        _tileIndicator.transform.localPosition = new Vector3(0f, 3f, 0f);
        _tileIndicator.transform.localScale = Vector3.one * 0.6f;

        if (_tileIndicator.TryGetComponent(out ObjectShaders objectShaders))
            objectShaders.GrayScale();

        _tileIndicator.transform.DOLocalMoveY(4f, 2f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);

        GetComponent<ObjectShaders>().Dissolve(3f, true);
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

    public void DestroySelf()
    {
        _tileIndicator.transform.DOKill();
        if (_tileIndicator.TryGetComponent(out ObjectShaders objectShaders))
            objectShaders.Dissolve(4f, false);

        GetComponent<ObjectShaders>().Dissolve(5f, false);
        Destroy(gameObject, 6f);
    }
}

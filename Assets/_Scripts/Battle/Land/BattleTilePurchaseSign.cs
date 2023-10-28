using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BattleTilePurchaseSign : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    BattleTooltipManager _tooltipManager;

    public BattleTile _tileToPurchase;

    GameObject _tileIndicator;

    bool _interactionBlocked;
    public event Action OnPurchased;
    public void Initialize(BattleTile tile)
    {
        _tooltipManager = BattleTooltipManager.Instance;

        _tileToPurchase = tile;

        StartCoroutine(InitializationCoroutine());
    }

    IEnumerator InitializationCoroutine()
    {
        GetComponent<ObjectShaders>().Dissolve(3f, true);
        yield return new WaitForSeconds(1.5f);
        HandleTileIndicator();
    }

    void HandleTileIndicator()
    {
        _tileIndicator = Instantiate(_tileToPurchase.TileIndicationPrefab, transform);
        _tileIndicator.transform.localPosition = new Vector3(0f, 3f, 0f);

        if (_tileIndicator.TryGetComponent(out ObjectShaders objectShaders))
            objectShaders.GrayScale();

        _tileIndicator.transform.DOLocalMoveY(4f, 2f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);

        _tileIndicator.transform.DORotate(new Vector3(0f, 360f, 0f), 8f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.Linear);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        if (_interactionBlocked) return;

        _tooltipManager.ShowHoverInfo(new BattleInfoElement("Enable Tile"));
        transform.DOShakePosition(0.5f, 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideHoverInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        PurchaseTile();
    }

    void PurchaseTile()
    {
        if (_interactionBlocked) return;

        _tileToPurchase.EnableTile();
        OnPurchased?.Invoke();
    }

    public void DestroySelf()
    {
        StartCoroutine(DestroySelfCoroutine());
    }

    IEnumerator DestroySelfCoroutine()
    {
        _interactionBlocked = true;
        _tileIndicator.transform.DOKill();
        yield return _tileIndicator.transform.DOScale(0, 0.5f).WaitForCompletion();

        GetComponent<ObjectShaders>().Dissolve(5f, false);
        Destroy(gameObject, 6f);

    }
}

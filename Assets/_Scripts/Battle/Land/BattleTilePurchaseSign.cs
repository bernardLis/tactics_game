using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BattleTilePurchaseSign : MonoBehaviour, IInteractable
{
    BattleTooltipManager _tooltipManager;

    [SerializeField] BattleFireFXController _fire;
    ObjectShaders _objectShaders;

    public BattleTile _tileToPurchase;

    GameObject _tileIndicator;

    bool _interactionBlocked;
    public event Action<BattleTile> OnPurchased;
    public void Initialize(BattleTile tile)
    {
        _tooltipManager = BattleTooltipManager.Instance;
        _objectShaders = GetComponent<ObjectShaders>();

        _tileToPurchase = tile;

        StartCoroutine(InitializationCoroutine());
    }

    IEnumerator InitializationCoroutine()
    {
        _objectShaders.Dissolve(3f, true);
        _fire.Activate();
        yield return new WaitForSeconds(1.5f);
        HandleTileIndicator();
    }

    void HandleTileIndicator()
    {
        _tileIndicator = Instantiate(_tileToPurchase.TileIndicationPrefab, transform);
        _tileIndicator.transform.localPosition = new Vector3(0f, 3f, 0f);
        Vector3 scale = _tileIndicator.transform.localScale;
        _tileIndicator.transform.localScale = Vector3.zero;
        _tileIndicator.transform.DOScale(scale, 0.5f).SetEase(Ease.OutBack);

        if (_tileIndicator.TryGetComponent(out ObjectShaders objectShaders))
            objectShaders.GrayScale();

        _tileIndicator.transform.DOLocalMoveY(3.5f, 2f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);

        _tileIndicator.transform.DORotate(new Vector3(0f, 360f, 0f), 8f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.Linear);
    }

    public void DisplayTooltip()
    {
        if (_tooltipManager == null) return;
        if (_interactionBlocked) return;

        _tooltipManager.ShowKeyTooltipInfo(new BattleInfoElement("Enable New Tile"));
    }

    public void HideTooltip()
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideKeyTooltipInfo();
    }

    public bool CanInteract(BattleInteractor interactor)
    {
        if (_interactionBlocked) return false;

        return true;
    }

    public bool Interact(BattleInteractor interactor)
    {
        if (_interactionBlocked) return false;

        PurchaseTile();
        return true;
    }

    void PurchaseTile()
    {
        if (_interactionBlocked) return;

        _tileToPurchase.EnableTile();
        OnPurchased?.Invoke(_tileToPurchase);
    }

    public void DestroySelf()
    {
        _interactionBlocked = true;
        StartCoroutine(DestroySelfCoroutine());
    }

    IEnumerator DestroySelfCoroutine()
    {
        _fire.Deactivate();
        _tileIndicator.transform.DOKill();
        GetComponent<BoxCollider>().enabled = false;
        yield return _tileIndicator.transform.DOScale(0, 0.5f).WaitForCompletion();

        _objectShaders.Dissolve(5f, false);
        Destroy(gameObject, 6f);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BattleBuilding : MonoBehaviour, IInteractable
{
    protected GameManager _gameManager;
    protected BattleManager _battleManager;
    protected BattleTooltipManager _tooltipManager;
    protected BattleFightManager _battleFightManager;

    [SerializeField] Transform _bannerSpawnPoint;
    GameObject _banner;

    protected Building _building;

    public virtual void Initialize(Building building)
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;
        _battleFightManager = BattleFightManager.Instance;
        _battleFightManager.OnFightEnded += Secured;

        _building = building;

        ShowBuilding();
    }

    protected virtual void ShowBuilding()
    {
        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(scale, 1f)
                .SetEase(Ease.OutBack)
                .SetDelay(2.5f);
        transform.DOLocalMoveY(scale.x * 0.5f, 1f)
            .SetEase(Ease.OutBack)
            .SetDelay(2.5f);

        transform.LookAt(_battleManager.GetComponent<BattleHeroManager>().BattleHero.transform.position);
    }

    protected virtual void Secured()
    {
        _banner = Instantiate(_gameManager.BannerPrefab, transform);
        _banner.transform.localPosition = _bannerSpawnPoint.localPosition;

        _building.Secure();
    }

    protected virtual void OnUpgradePurchased()
    {
    }

    /* INTERACTION */
    public bool CanInteract(BattleInteractor interactor)
    {
        return _building.IsSecured;
    }

    public virtual void DisplayTooltip()
    {
        if (_tooltipManager == null) return;

        _tooltipManager.ShowTooltip(new BuildingCard(_building), gameObject);

    }

    public void HideTooltip()
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideKeyTooltipInfo();
        _tooltipManager.HideTooltip();
    }

    public virtual bool Interact(BattleInteractor interactor)
    {

        return true;
    }

}

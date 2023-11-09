using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class BattleBuilding : MonoBehaviour, IInteractable
{
    protected GameManager _gameManager;
    protected BattleManager _battleManager;
    protected BattleTooltipManager _tooltipManager;
    protected BattleFightManager _battleFightManager;

    [SerializeField] GameObject _corruptionEffectPrefab;
    GameObject _buildingCorruptionEffect;

    [SerializeField] Transform _bannerSpawnPoint;
    GameObject _banner;

    protected Building _building;

    IEnumerator _corruptionCoroutine;

    public event Action OnBuildingCorrupted;
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

    public void GetCorrupted(BattleBoss boss)
    {
        boss.OnCorruptionBroken += BreakCorruption;
        _corruptionCoroutine = CorruptionCoroutine();
        StartCoroutine(_corruptionCoroutine);

    }

    IEnumerator CorruptionCoroutine()
    {
        yield return DisplayCorruptionEffect();
        for (int i = 0; i < 10; i++)
        {
            // HERE: display corruption progress bar
            Debug.Log($"building corrupted in {10 - i} seconds");
            yield return new WaitForSeconds(1);
        }
        yield return HideCorruptionEffect();
        Corrupted();
    }

    protected virtual void BreakCorruption()
    {
        StartCoroutine(HideCorruptionEffect());
    }

    IEnumerator DisplayCorruptionEffect()
    {
        Vector3 pos = transform.position;
        pos.y = 0.02f;
        _buildingCorruptionEffect = Instantiate(_corruptionEffectPrefab, pos, Quaternion.identity);
        _buildingCorruptionEffect.transform.localScale = Vector3.zero;
        float scale = transform.localScale.x;
        yield return _buildingCorruptionEffect.transform.DOScale(scale, 0.5f)
                                              .SetEase(Ease.OutBack)
                                              .WaitForCompletion();

        _buildingCorruptionEffect.transform.DORotate(new Vector3(0, 360, 0), 10f, RotateMode.FastBeyond360)
                                           .SetRelative(true)
                                           .SetLoops(-1, LoopType.Incremental)
                                           .SetEase(Ease.InOutSine);
    }

    IEnumerator HideCorruptionEffect()
    {
        if (_buildingCorruptionEffect == null) yield break;
        _buildingCorruptionEffect.transform.DOKill();
        yield return _buildingCorruptionEffect.transform.DOScale(0, 1f)
                                            .SetEase(Ease.InBack)
                                            .OnComplete(() => Destroy(_buildingCorruptionEffect))
                                            .WaitForCompletion();
    }


    public virtual void Corrupted()
    {
        _banner.transform.DOScale(0, 1.5f)
                 .SetEase(Ease.InBack)
                 .OnComplete(() => Destroy(_banner));
        _building.Corrupted();
        OnBuildingCorrupted?.Invoke();
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

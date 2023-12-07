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
    protected BattleMinionManager _battleMinionManager;

    [SerializeField] GameObject _corruptionEffectPrefab;
    GameObject _buildingCorruptionEffect;

    protected Building _building;

    protected ProgressBarHandler _progressBarHandler;

    protected IEnumerator _corruptionCoroutine;
    protected bool _corruptionPaused;

    public event Action OnBuildingCorrupted;
    public virtual void Initialize(Vector3 pos, Building building)
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;
        _battleMinionManager = BattleMinionManager.Instance;

        _building = building;

        transform.localPosition = pos;

        _progressBarHandler = GetComponentInChildren<ProgressBarHandler>();
        _progressBarHandler.Initialize();
        _progressBarHandler.HideProgressBar();

        StartCoroutine(SecuredCoroutine());
    }


    public virtual IEnumerator SecuredCoroutine()
    {
        yield return ShowBuilding();

        _building.Secure();
    }

    protected virtual IEnumerator ShowBuilding()
    {
        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.LookAt(_battleManager.GetComponent<BattleHeroManager>().BattleHero.transform.position);

        transform.DOScale(scale, 1f)
                .SetEase(Ease.OutBack);
        yield return transform.DOLocalMoveY(transform.localPosition.y + scale.x * 0.5f, 1f)
                .SetEase(Ease.OutBack)
                .WaitForCompletion();

    }

    public virtual void StartCorruption(BattleBoss boss)
    {
        boss.OnCorruptionBroken += BreakCorruption;
        _corruptionCoroutine = CorruptionCoroutine();
        StartCoroutine(_corruptionCoroutine);
    }

    protected void PauseCorruption()
    {
        _corruptionPaused = true;
        DOTween.Pause("CorruptionEffectRotation");
    }

    protected void ResumeCorruption()
    {
        _corruptionPaused = false;
        DOTween.Play("CorruptionEffectRotation");
    }

    protected IEnumerator CorruptionCoroutine()
    {
        yield return DisplayCorruptionEffect();
        Color c = _gameManager.GameDatabase.GetColorByName("Corruption").Color;
        _progressBarHandler.SetFillColor(c);
        _progressBarHandler.SetBorderColor(Color.black);
        _progressBarHandler.SetProgress(0);
        _progressBarHandler.ShowProgressBar();

        int totalSecondsToCorrupt = _building.SecondsToCorrupt +
                    _gameManager.GlobalUpgradeBoard.BossCorruptionDuration.GetValue();

        for (int i = 0; i <= totalSecondsToCorrupt; i++)
        {
            if (_corruptionPaused) yield return new WaitUntil(() => !_corruptionPaused);
            yield return new WaitForSeconds(1);
            _progressBarHandler.SetProgress((float)i / totalSecondsToCorrupt);
        }

        yield return HideCorruptionEffect();
        Corrupted();
    }

    protected virtual void BreakCorruption()
    {
        if (_corruptionCoroutine != null)
            StopCoroutine(_corruptionCoroutine);
        _corruptionCoroutine = null;

        _progressBarHandler.HideProgressBar();
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
                                           .SetEase(Ease.InOutSine)
                                           .SetId("CorruptionEffectRotation");
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
        _building.Corrupted();
        OnBuildingCorrupted?.Invoke();
    }

    /* INTERACTION */
    public virtual bool CanInteract(BattleInteractor interactor)
    {
        if (_corruptionCoroutine != null) return false;
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

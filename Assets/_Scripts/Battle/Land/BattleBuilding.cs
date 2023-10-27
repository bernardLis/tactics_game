using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BattleBuilding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    protected GameManager _gameManager;
    protected BattleManager _battleManager;
    protected BattleTooltipManager _tooltipManager;
    protected BattleFightManager _battleFightManager;

    [SerializeField] GameObject _banner;

    protected Building _building;

    protected IEnumerator _productionCoroutine;
    float _currentProductionDelaySecond;

    public virtual void Initialize(Building building)
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;
        _battleFightManager = BattleFightManager.Instance;
        _battleFightManager.OnWaveSpawned += SpawnWave;
        _battleFightManager.OnFightEnded += Secured;

        _building = building;
    }

    public virtual void SpawnWave()
    {
        // meant to be overwritten
    }

    public void Secured()
    {
        _battleFightManager.OnWaveSpawned -= SpawnWave;

        _banner.SetActive(true);
        _building.Secure();
        StartProductionCoroutine();
    }

    protected void StartProductionCoroutine()
    {
        if (!_building.IsSecured) return;
        if (_productionCoroutine != null) return;

        _productionCoroutine = ProductionCoroutine();
        StartCoroutine(_productionCoroutine);
    }

    protected virtual IEnumerator ProductionCoroutine()
    {
        // meant to be overwritten
        yield return null;
    }

    protected IEnumerator ProductionDelay()
    {
        float totalDelay = _building.GetCurrentUpgrade().ProductionDelay;
        _currentProductionDelaySecond = 0f;
        while (_currentProductionDelaySecond < totalDelay)
        {
            _currentProductionDelaySecond += 1;
            yield return new WaitForSeconds(1f);
        }
    }

    /* Mouse */
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowHoverInfo(
                    new BattleInfoElement($"<b>{Helpers.ParseScriptableObjectName(_building.name)}</b>"));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideHoverInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        _tooltipManager.ShowTooltip(new BuildingCard(_building), gameObject);
    }

}

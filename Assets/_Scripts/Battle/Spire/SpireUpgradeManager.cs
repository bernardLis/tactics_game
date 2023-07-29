using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SpireUpgradeManager : Singleton<SpireUpgradeManager>, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    Spire _spire;
    SpireElement _spireElement;

    public event Action<BattleTurret> OnTurretAdded;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _tooltipManager = BattleTooltipManager.Instance;

        _spire = _gameManager.SelectedBattle.Spire;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo($"Click for base upgrades");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_spire == null) _spire = _gameManager.SelectedBattle.Spire;

        _spireElement = new SpireElement(_spire);
        _spireElement.OnStoreyPurchased += ResolveStoreyPurchased;
        _spireElement.style.opacity = 0;
        _battleManager.Root.Add(_spireElement);
        _battleManager.PauseGame();
        DOTween.To(x => _spireElement.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

        _spireElement.OnClosed += () =>
        {
            _battleManager.ResumeGame();
            _battleManager.Root.Remove(_spireElement);
            _spireElement = null;
        };
    }

    void ResolveStoreyPurchased(Storey storey)
    {
        if (storey is StoreyTurret)
        {
            StoreyTurret st = storey as StoreyTurret;
            InstantiateTurret(st);
        }
    }

    public void InstantiateTurret(StoreyTurret st)
    {
        Turret scriptableObjectInstance = Instantiate(st.TurretOriginal);
        GameObject gameObjectInstance = Instantiate(scriptableObjectInstance.Prefab, _battleManager.EntityHolder);
        gameObjectInstance.GetComponent<TurretStoreyManager>().Initialize(st);
        BattleTurret instance = gameObjectInstance.GetComponent<BattleTurret>();
        instance.Initialize(scriptableObjectInstance);

        gameObjectInstance.transform.position = new Vector3(0, 0, 2);
        OnTurretAdded?.Invoke(instance);
    }

}
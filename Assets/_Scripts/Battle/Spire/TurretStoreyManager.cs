using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TurretStoreyManager : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTurretsManager _battleTurretsManager;
    BattleTooltipManager _tooltipManager;

    Spire _base;
    StoreyTurret _storeyTurret;
    StoreyTurretElement _storeyTurretElement;

    [SerializeField] Element _element;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleTurretsManager = _battleManager.GetComponent<BattleTurretsManager>();

        _battleTurretsManager.InstantiateTurret(_element);

        _tooltipManager = BattleTooltipManager.Instance;

        _base = _gameManager.SelectedBattle.Spire;
        _storeyTurret = _base.StoreyTurrets.Find(x => x.Element == _element);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo($"Click for turret upgrades");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Turret upgrades");

        _storeyTurretElement = new(_storeyTurret);
        _storeyTurretElement.style.opacity = 0;
        _battleManager.Root.Add(_storeyTurretElement);
        _battleManager.PauseGame();
        DOTween.To(x => _storeyTurretElement.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

        _storeyTurretElement.OnClosed += () =>
        {
            _battleManager.ResumeGame();
            _battleManager.Root.Remove(_storeyTurretElement);
            _storeyTurretElement = null;
        };
    }
}

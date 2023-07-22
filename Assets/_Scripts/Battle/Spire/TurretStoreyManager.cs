using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TurretStoreyManager : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    Spire _base;
    StoreyTurretsElement _storeyTurretsElement;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _tooltipManager = BattleTooltipManager.Instance;

        _base = _gameManager.SelectedBattle.Spire;
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

        _storeyTurretsElement = new(_base.StoreyTurrets);
        _storeyTurretsElement.style.opacity = 0;
        _battleManager.Root.Add(_storeyTurretsElement);
        _battleManager.PauseGame();
        DOTween.To(x => _storeyTurretsElement.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

        _storeyTurretsElement.OnClosed += () =>
        {
            _battleManager.ResumeGame();
            _battleManager.Root.Remove(_storeyTurretsElement);
            _storeyTurretsElement = null;
        };
    }
}

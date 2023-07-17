using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TroopsStoreyManager : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    Spire _base;
    SpireElement _baseElement;

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
        _tooltipManager.ShowInfo($"Click for troops upgrades");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Base troops upgrade");
        _baseElement = new SpireElement(_base);
        _baseElement.style.opacity = 0;
        _battleManager.Root.Add(_baseElement);
        _battleManager.PauseGame();
        DOTween.To(x => _baseElement.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

        _baseElement.OnClosed += () =>
        {
            _battleManager.ResumeGame();
            _battleManager.Root.Remove(_baseElement);
            _baseElement = null;
        };
    }
}

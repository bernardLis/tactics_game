using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SpireUpgradeManager : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    Spire _spire;
    SpireElement _spireElement;

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
        if (_spire == null) _spire = _gameManager.SelectedBattle.Spire;

        _spireElement = new SpireElement(_spire);
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
}
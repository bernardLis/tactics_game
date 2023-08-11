using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class LivesStoreyManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    GameManager _gameManager;
    BattleTooltipManager _tooltipManager;

    Spire _spire;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _tooltipManager = BattleTooltipManager.Instance;

        _spire = _gameManager.SelectedBattle.Spire;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo($"Click for lives upgrades");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        new StoreyLivesElement(_spire.StoreyLives);
    }
}

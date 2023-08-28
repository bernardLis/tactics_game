using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class LivesStoreyManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    GameManager _gameManager;
    BattleTooltipManager _tooltipManager;
    BattleSpireLight _battleSpireLight;

    Spire _spire;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _tooltipManager = BattleTooltipManager.Instance;

        _spire = BattleSpire.Instance.Spire;

        _battleSpireLight = GetComponentInChildren<BattleSpireLight>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo($"Click for lives upgrades");
        _battleSpireLight.PauseLight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
        _battleSpireLight.ResumeLight();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_spire == null) _spire = BattleSpire.Instance.Spire;

        new StoreyLivesElement(_spire.StoreyLives);
    }
}

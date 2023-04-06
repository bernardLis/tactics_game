using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCastle : MonoBehaviour, ITooltipDisplayable, IPointerEnterHandler, IPointerExitHandler
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;
    DraggableArmies _draggableArmies;

    SpriteOutline _spriteOutline;
    MapTooltipDisplayer _tooltipDisplayer;

    public Castle Castle { get; private set; }

    public event Action<MapCastle> PointerEnterEvent;
    public event Action<MapCastle> PointerLeaveEvent;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _dashboardManager = DashboardManager.Instance;
        _draggableArmies = _dashboardManager.GetComponent<DraggableArmies>();

        _spriteOutline = GetComponent<SpriteOutline>();
        _tooltipDisplayer = GetComponent<MapTooltipDisplayer>();
    }

    public void Initialize(Castle castle)
    {
        castle.Initialize(); // TODO: differentiate between owned / not owned castles
        Castle = castle;
        GetComponentInChildren<SpriteRenderer>().sprite = castle.Sprite;
    }

    public void OnPointerEnter(PointerEventData evt)
    {
        PointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData evt)
    {
        PointerLeaveEvent?.Invoke(this);
    }

    public void Highlight()
    {
        _spriteOutline.Highlight();
        _tooltipDisplayer.DisplayTooltip();
    }

    public void ClearHighlight()
    {
        _spriteOutline.ClearHighlight();
        _tooltipDisplayer.HideTooltip();
    }

    public void ShowCastleUI()
    {
        CastleElement el = new(_dashboardManager.Root, Castle, null);
        el.OnHide += _draggableArmies.Reset;
        _draggableArmies.Initialize();
    }

    public void VisitCastle(MapHero h)
    {
        Debug.Log($"{h.Character.CharacterName} is visiting {Castle.name}");

        CastleElement el = new(_dashboardManager.Root, Castle, h);
        el.OnHide += _draggableArmies.Reset;
        _draggableArmies.Initialize();
    }

    public string GetTooltipText() { return Castle.name; }
}

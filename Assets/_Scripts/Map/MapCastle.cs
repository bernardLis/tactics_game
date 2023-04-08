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
    MapInputManager _mapInputManager;

    SpriteOutline _spriteOutline;
    MapTooltipDisplayer _tooltipDisplayer;

    public Castle Castle { get; private set; }

    public bool IsSelected { get; private set; }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _dashboardManager = DashboardManager.Instance;
        _draggableArmies = _dashboardManager.GetComponent<DraggableArmies>();
        _mapInputManager = MapInputManager.Instance;
        _mapInputManager.OnCastleSelected += c =>
        {
            if (c != this)
                return;

            IsSelected = true;
            Highlight();
        };

        _mapInputManager.OnCastleUnselected += c =>
        {
            if (c != this)
                return;

            IsSelected = false;
            ClearHighlight();
        };

        _spriteOutline = GetComponent<SpriteOutline>();
        _tooltipDisplayer = GetComponent<MapTooltipDisplayer>();
    }

    public void Initialize(Castle castle)
    {
        castle.Initialize(); // TODO: differentiate between owned / not owned castles
        Castle = castle;
        GetComponentInChildren<SpriteRenderer>().sprite = castle.Sprite;
    }

    public void OnPointerEnter(PointerEventData evt) { Select(); }

    public void OnPointerExit(PointerEventData evt) { Unselect(); }

    public void Select()
    {
        _mapInputManager.SelectCastle(this);
        IsSelected = true;
    }

    public void Unselect()
    {
        _mapInputManager.UnselectCastle(this);
        IsSelected = false;
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
        Debug.Log($"{h.Hero.HeroName} is visiting {Castle.name}");

        CastleElement el = new(_dashboardManager.Root, Castle, h);
        el.OnHide += _draggableArmies.Reset;
        _draggableArmies.Initialize();
    }

    public string GetTooltipText() { return Castle.name; }
}

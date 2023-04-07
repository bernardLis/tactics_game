using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CastleControlButton : ControlButton
{
    const string _ussClassName = "castle-control-button__";

    const string _ussCheckMark = _ussClassName + "check-mark";

    public MapCastle MapCastle { get; private set; }
    VisualElement _builtCheckMark;

    public CastleControlButton(MapCastle mapCastle, VisualElement root, DraggableArmies draggableArmies) : base(root, draggableArmies)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CastleControlButtonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        MapCastle = mapCastle;
        mapCastle.Castle.OnBuilt += OnBuilt;


        _mapInputManager.OnCastleSelected += c =>
        {
            if (c != MapCastle)
                return;

            AddToClassList(_ussSelected);
        };
        _mapInputManager.OnCastleUnselected += c =>
        {
            if (c != MapCastle)
                return;

            RemoveFromClassList(_ussSelected);
        };

        _icon.style.backgroundImage = new StyleBackground(mapCastle.Castle.Sprite);

        AddBuiltCheckMark();
    }

    void AddBuiltCheckMark()
    {
        _builtCheckMark = new VisualElement();
        _builtCheckMark.AddToClassList(_ussCheckMark);
        Add(_builtCheckMark);

        if (!MapCastle.Castle.HasBuiltToday)
            _builtCheckMark.style.display = DisplayStyle.None;
    }

    void OnBuilt() { _builtCheckMark.style.display = DisplayStyle.Flex; }

    protected override void OnDayPassed(int day) { _builtCheckMark.style.display = DisplayStyle.None; }

    protected override void OnPointerDown(PointerDownEvent e)
    {
        if (e.button == 1) return; // right mouse click

        Select();
    }

    void Select()
    {
        if (MapCastle.IsSelected)
        {
            CastleElement el = new(_root, MapCastle.Castle);
            el.OnHide += () =>
            {
                RemoveFromClassList(_ussSelected);
                _draggableArmies.Reset();
            };
            _draggableArmies.Initialize();
            return;
        }
        _cameraSmoothFollow.MoveTo(MapCastle.transform.position);
        _mapInputManager.SelectCastle(MapCastle);
    }


    protected override void OnMouseEnter(MouseEnterEvent e)
    {
    }

    protected override void OnMouseLeave(MouseLeaveEvent e)
    {
    }

}

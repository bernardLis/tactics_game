using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CastleControlsButton : ControlsButton
{
    const string _ussClassName = "castle-controls-button__";
    const string _ussCheckMark = _ussClassName + "check-mark";

    public MapCastle MapCastle { get; private set; }
    VisualElement _builtCheckMark;

    public CastleControlsButton(MapCastle mapCastle, VisualElement root, DraggableArmies draggableArmies) : base(root, draggableArmies)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CastleControlsButtonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        MapCastle = mapCastle;
        mapCastle.Castle.OnBuilt += OnBuilt;

        style.backgroundImage = new StyleBackground(mapCastle.Castle.Sprite);

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
        CastleElement el = new(_root, MapCastle.Castle);
        el.OnHide += _draggableArmies.Reset;
        _draggableArmies.Initialize();
    }

    protected override void OnMouseEnter(MouseEnterEvent e)
    {
        _cameraSmoothFollow.MoveTo(MapCastle.transform.position);
        MapCastle.Highlight();
    }

    protected override void OnMouseLeave(MouseLeaveEvent e)
    {
        MapCastle.ClearHighlight();
    }

}

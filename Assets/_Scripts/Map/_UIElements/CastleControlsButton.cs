using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CastleControlsButton : ControlsButton
{
    public MapCastle MapCastle { get; private set; }

    public CastleControlsButton(MapCastle mapCastle, VisualElement root, DraggableArmies draggableArmies) : base(root, draggableArmies)
    {
        MapCastle = mapCastle;
        style.backgroundImage = new StyleBackground(mapCastle.Castle.Sprite);
    }

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

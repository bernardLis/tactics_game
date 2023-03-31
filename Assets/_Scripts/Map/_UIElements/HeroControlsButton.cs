using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroControlsButton : ControlsButton
{

    public MapHero MapHero { get; private set; }

    public HeroControlsButton(MapHero mapHero, VisualElement root, DraggableArmies draggableArmies) : base(root, draggableArmies)
    {
        MapHero = mapHero;
        style.backgroundImage = new StyleBackground(mapHero.Character.Portrait.Sprite);
    }

    protected override void OnPointerDown(PointerDownEvent e)
    {
        Debug.Log($"{MapHero.Character.CharacterName} click show me a full screen card with all info");
    }

    protected override void OnMouseEnter(MouseEnterEvent e)
    {
        _cameraSmoothFollow.MoveTo(MapHero.transform.position);
    }

    protected override void OnMouseLeave(MouseLeaveEvent e)
    {
    }


}

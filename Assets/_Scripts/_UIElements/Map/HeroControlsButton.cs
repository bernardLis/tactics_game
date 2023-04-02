using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroControlsButton : ControlsButton
{

    public MapHero MapHero { get; private set; }

    ResourceBarElement _movementRangeBar;

    public HeroControlsButton(MapHero mapHero, VisualElement root, DraggableArmies draggableArmies) : base(root, draggableArmies)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroControlsButtonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        MapHero = mapHero;
        style.backgroundImage = new StyleBackground(mapHero.Character.Portrait.Sprite);

        AddMovementRangeBar();
    }

    void AddMovementRangeBar()
    {
        _movementRangeBar = new(Color.green, "Movement Range",
             totalValueStat: MapHero.Character.Speed, currentFloatVar: MapHero.RangeLeft);
        Add(_movementRangeBar);
    }

    protected override void OnPointerDown(PointerDownEvent e)
    {
        Debug.Log($"{MapHero.Character.CharacterName} click show me a full screen card with all info");
        HeroCardElement card = new(MapHero, _root, _draggableArmies);
    }

    protected override void OnMouseEnter(MouseEnterEvent e)
    {
        _cameraSmoothFollow.MoveTo(MapHero.transform.position);
    }

    protected override void OnMouseLeave(MouseLeaveEvent e)
    {
    }


}

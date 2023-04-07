using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroControlButton : ControlButton
{
    const string _ussClassName = "hero-controls-button__";

    public MapHero MapHero { get; private set; }

    ResourceBarElement _movementRangeBar;

    public HeroControlButton(MapHero mapHero, VisualElement root, DraggableArmies draggableArmies) : base(root, draggableArmies)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroControlButtonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _mapInputManager.OnHeroSelected += h =>
        {
            if (h == MapHero)
                AddToClassList(_ussSelected);
        };

        _mapInputManager.OnHeroUnselected += h =>
        {
            if (h == MapHero)
                RemoveFromClassList(_ussSelected);
        };


        MapHero = mapHero;

        _icon.style.backgroundImage = new StyleBackground(mapHero.Character.Portrait.Sprite);

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
        if (_mapInputManager.SelectedHero == MapHero)
        {
            HeroCardElement card = new(MapHero, _root, _draggableArmies);
            return;
        }

        AddToClassList(_ussSelected);
        _mapInputManager.SelectHero(MapHero);
    }

    protected override void OnMouseEnter(MouseEnterEvent e) { }

    protected override void OnMouseLeave(MouseLeaveEvent e) { }


}

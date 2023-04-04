using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroControlsButton : ControlsButton
{
    MapInputManager _mapInputManager;
    public MapHero MapHero { get; private set; }

    ResourceBarElement _movementRangeBar;

    public HeroControlsButton(MapHero mapHero, VisualElement root, DraggableArmies draggableArmies) : base(root, draggableArmies)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroControlsButtonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _mapInputManager = MapInputManager.Instance;

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
        if (_mapInputManager.SelectedHero == MapHero)
        {
            HeroCardElement card = new(MapHero, _root, _draggableArmies);
            return;
        }

        _mapInputManager.SelectHero(MapHero);
    }

    protected override void OnMouseEnter(MouseEnterEvent e)
    {
        //_cameraSmoothFollow.MoveTo(MapHero.transform.position);
    }

    protected override void OnMouseLeave(MouseLeaveEvent e)
    {
    }


}

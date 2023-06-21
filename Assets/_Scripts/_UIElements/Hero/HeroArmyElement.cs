using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroArmyElement : VisualElement
{
    const string _ussClassName = "hero-army__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    public Hero Hero;

    ScrollView _armySlotScrollView;

    List<ArmySlotElement> _armySlots = new();

    public HeroArmyElement(Hero hero)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroArmyElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Hero = hero;

        AddToClassList(_ussMain);

        _armySlotScrollView = new ScrollView();
        _armySlotScrollView.contentContainer.style.flexDirection = FlexDirection.Row;
        Add(_armySlotScrollView);

        for (int i = 0; i < Hero.Army.Count; i++)
        {
            ArmySlotElement armySlot = new(null, i);
            _armySlotScrollView.Add(armySlot);
            _armySlots.Add(armySlot);
        }

        for (int i = 0; i < Hero.Army.Count; i++)
            _armySlots[i].AddArmyNoDelegates(new(Hero.Army[i]));
    }
}
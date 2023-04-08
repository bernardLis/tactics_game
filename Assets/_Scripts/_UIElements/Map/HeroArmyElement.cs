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

    List<ArmySlotElement> _armySlotElements = new();

    public HeroArmyElement(Hero hero, bool isCardDisplayed = true)
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
        
        if (isCardDisplayed)
            Add(new HeroCardMini(Hero));

        _armySlotElements = new();
        for (int i = 0; i < Hero.MaxHeroArmySlots; i++)
        {
            ArmySlotElement el = new(null, i);
            _armySlotElements.Add(el);
            el.OnArmyAdded += (ArmyElement el) => Hero.AddArmy(el.ArmyGroup);
            el.OnArmyRemoved += (ArmyElement el) => Hero.RemoveArmy(el.ArmyGroup);
            Add(el);
        }

        foreach (ArmyGroup ag in Hero.Army)
            _armySlotElements[ag.ListPosition].AddArmyNoDelegates(new(ag));
    }


}

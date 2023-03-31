using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroCastleElement : VisualElement
{
    const string _ussClassName = "hero-castle__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    public Character Character;

    List<ArmySlotElement> _armySlotElements = new();

    public HeroCastleElement(Character character, bool isCardDisplayed = true)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroCastleElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Character = character;

        AddToClassList(_ussMain);
        
        if (isCardDisplayed)
            Add(new CharacterCardMini(Character));

        _armySlotElements = new();
        for (int i = 0; i < Character.MaxCharacterArmySlots; i++)
        {
            ArmySlotElement el = new(null, i);
            _armySlotElements.Add(el);
            el.OnArmyAdded += (ArmyElement el) => Character.AddArmy(el.ArmyGroup);
            el.OnArmyRemoved += (ArmyElement el) => Character.RemoveArmy(el.ArmyGroup);
            Add(el);
        }

        foreach (ArmyGroup ag in Character.Army)
            _armySlotElements[ag.ListPosition].AddArmyNoDelegates(new(ag));
    }


}

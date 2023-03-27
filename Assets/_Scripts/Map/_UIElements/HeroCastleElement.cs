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

    public HeroCastleElement(Character character)
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

        Add(new CharacterCardMini(Character));
        _armySlotElements = new();
        for (int i = 0; i < Character.MaxCharacterArmySlots; i++)
        {
            ArmySlotElement el = new();
            _armySlotElements.Add(el);
            el.OnArmyAdded += (ArmyElement el) => Character.AddArmy(el.ArmyGroup);
            el.OnArmyAdded += (ArmyElement el) => Character.RemoveArmy(el.ArmyGroup);
            Add(el);
        }

        for (int i = 0; i < Character.ArmyGroups.Count; i++)
        {
            _armySlotElements[i].AddArmyNoDelegates(new(Character.ArmyGroups[i]));
        }
    }


}

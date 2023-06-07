using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmySlotElement : ElementWithSound
{
    const string _ussClassName = "army-slot";
    const string _ussMain = _ussClassName + "__main";

    GameManager _gameManager;

    public ArmyGroupElement ArmyElement;

    public bool IsLocked { get; private set; }
    public int ListPosition { get; private set; }

    public event Action<ArmyGroupElement> OnArmyAdded;
    public event Action<ArmyGroupElement> OnArmyRemoved;
    public ArmySlotElement(ArmyGroupElement armyElement = null, int listPosition = 0, bool isLocked = false) : base()
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ArmySlotElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        RegisterCallback<MouseEnterEvent>((evt) => PlayClick());
        AddToClassList(_ussMain);

        IsLocked = isLocked;
        ListPosition = listPosition;

        if (armyElement == null)
            return;

        ArmyElement = armyElement;
        Add(armyElement);
    }

    public void AddArmy(ArmyGroupElement armyElement)
    {
        if (armyElement.ArmyGroup.NumberOfUnits == 0) return;

        bool callDelegate = true;
        // gonna be merged
        if (ArmyElement != null && ArmyElement.ArmyGroup.Creature == armyElement.ArmyGroup.Creature)
            callDelegate = false;

        AddArmyNoDelegates(armyElement);
        PlayClick();

        if (callDelegate)
            OnArmyAdded?.Invoke(armyElement);
    }

    public void AddArmyNoDelegates(ArmyGroupElement armyElement)
    {
        armyElement.ArmyGroup.ListPosition = ListPosition;

        // merge if there is already the same army entity
        if (ArmyElement != null && ArmyElement.ArmyGroup.Creature == armyElement.ArmyGroup.Creature)
        {
            ArmyElement.ArmyGroup.ChangeCount(armyElement.ArmyGroup.NumberOfUnits);
            return;
        }

        ArmyElement = armyElement;
        armyElement.style.position = Position.Relative;
        Add(armyElement);
    }

    public void RemoveArmy()
    {
        OnArmyRemoved?.Invoke(ArmyElement);
        Clear();
        ArmyElement = null;
    }
}

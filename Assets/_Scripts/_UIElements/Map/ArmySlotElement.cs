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

    public ArmyElement ArmyElement;

    public bool IsLocked { get; private set; }
    public int ListPosition { get; private set; }

    public event Action<ArmyElement> OnArmyAdded;
    public event Action<ArmyElement> OnArmyRemoved;
    public ArmySlotElement(ArmyElement armyElement = null, int listPosition = 0, bool isLocked = false) : base()
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

    public void AddArmy(ArmyElement armyElement)
    {
        if (armyElement.ArmyGroup.EntityCount == 0) return;

        bool callDelegate = true;
        // gonna be merged
        if (ArmyElement != null && ArmyElement.ArmyGroup.ArmyEntity == armyElement.ArmyGroup.ArmyEntity)
            callDelegate = false;

        AddArmyNoDelegates(armyElement);
        PlayClick();

        if (callDelegate)
            OnArmyAdded?.Invoke(armyElement);
    }

    public void AddArmyNoDelegates(ArmyElement armyElement)
    {
        armyElement.ArmyGroup.ListPosition = ListPosition;

        // merge if there is already the same army entity
        if (ArmyElement != null && ArmyElement.ArmyGroup.ArmyEntity == armyElement.ArmyGroup.ArmyEntity)
        {
            ArmyElement.ArmyGroup.ChangeCount(armyElement.ArmyGroup.EntityCount);
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

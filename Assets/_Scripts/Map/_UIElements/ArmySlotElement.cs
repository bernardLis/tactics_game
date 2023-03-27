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

    public event Action<ArmyElement> OnArmyAdded;
    public event Action<ArmyElement> OnArmyRemoved;
    public ArmySlotElement(ArmyElement armyElement = null) : base()
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ArmySlotElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        RegisterCallback<MouseEnterEvent>((evt) => PlayClick());
        AddToClassList(_ussMain);

        if (armyElement == null)
            return;

        ArmyElement = armyElement;
        Add(armyElement);
    }

    public void AddArmy(ArmyElement armyElement)
    {
        if (armyElement.ArmyGroup.EntityCount == 0)
            return;

        AddArmyNoDelegates(armyElement);
        PlayClick();
        OnArmyAdded?.Invoke(armyElement);
    }

    public void AddArmyNoDelegates(ArmyElement armyElement)
    {
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

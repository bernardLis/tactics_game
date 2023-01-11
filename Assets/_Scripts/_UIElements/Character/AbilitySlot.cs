using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilitySlot : ElementWithSound
{

    public AbilityButton AbilityButton;
    public Character Character;

    const string _ussClassName = "ability-slot";
    const string _ussMain = _ussClassName + "__main";

    public event Action<AbilityButton> OnAbilityAdded;
    public event Action<AbilityButton> OnAbilityRemoved;
    public AbilitySlot(AbilityButton abilityButton = null) : base()
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.AbilitySlotStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        if (abilityButton == null)
            return;

        AbilityButton = abilityButton;
        Add(abilityButton);
    }

    public void AddButton(AbilityButton abilityButton)
    {
        AddButtonNoDelegates(abilityButton);
        PlayClick();
        OnAbilityAdded?.Invoke(abilityButton);
    }

    public void AddButtonNoDelegates(AbilityButton abilityButton)
    {
        AbilityButton = abilityButton;
        PlayClick();
        abilityButton.style.position = Position.Relative;
        Add(abilityButton);
    }

    public void RemoveButton()
    {
        OnAbilityRemoved?.Invoke(AbilityButton);
        Clear();
        AbilityButton = null;
    }
}

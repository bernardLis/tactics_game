using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilitySlot : ElementWithSound
{

    public AbilityButton AbilityButton;
    public Ability Ability;
    public Hero Hero;

    const string _ussClassName = "ability-slot";
    const string _ussMain = _ussClassName + "__main";

    public event Action<Ability> OnAbilityAdded;
    public event Action<Ability> OnAbilityRemoved;
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

    public void AddActionButton(AbilityButton abilityButton)
    {
        Add(AbilityButton);
    }

    public void AddDraggableButton(Ability ability) // TODO: ugh oh... does not seem correct
    {
        AddDraggableButtonNoDelegates(ability);
        PlayClick();
        OnAbilityAdded?.Invoke(ability);
    }

    public void AddDraggableButtonNoDelegates(Ability ability)
    {
        Ability = ability;
        AbilityButton = new(ability);

        PlayClick();
        AbilityButton.style.position = Position.Relative;
        Add(AbilityButton);
    }

    public void RemoveButton()
    {
        OnAbilityRemoved?.Invoke(Ability);
        Clear();
        AbilityButton = null;
        Ability = null;
    }
}

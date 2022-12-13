using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilitySlot : ElementWithSound
{

    public AbilityButton AbilityButton;
    public Character Character;

    public AbilitySlot(AbilityButton abilityButton = null) : base()
    {
        AddToClassList("abilitySlot");

        if (abilityButton == null)
            return;

        AbilityButton = abilityButton;
        Add(abilityButton);
    }

    public void AddButton(AbilityButton abilityButton)
    {
        AbilityButton = abilityButton;
        Add(abilityButton);
        PlayClick();
    }

    public void RemoveButton()
    {
        Clear();
        AbilityButton = null;
    }
}

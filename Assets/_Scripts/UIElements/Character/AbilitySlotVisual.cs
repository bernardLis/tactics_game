using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilitySlotVisual : VisualElement
{
    public AbilityButton AbilityButton;
    public Character Character;

    public AbilitySlotVisual(AbilityButton abilityButton = null)
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
    }

}

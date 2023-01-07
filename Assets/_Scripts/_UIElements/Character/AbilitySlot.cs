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

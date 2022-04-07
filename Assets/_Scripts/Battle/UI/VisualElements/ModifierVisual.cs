using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ModifierVisual : VisualElement
{
    public StatModifier Modifier;
    public Status Status;

    public ModifierVisual(StatModifier statModifier)
    {
        BaseModifierVisual();
        style.backgroundImage = statModifier.Icon.texture;
    }

    public ModifierVisual(Status status)
    {
        BaseModifierVisual();
        style.backgroundImage = status.Icon.texture;
    }

    void BaseModifierVisual()
    {
        AddToClassList("modifierIcon");
    }
}

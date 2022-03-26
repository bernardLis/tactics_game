using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class BattleUIHelpers
{
    public static void HandleStatCheck(Stat stat, Label label)
    {
        label.style.color = Color.white;
        if (stat.GetValue() > stat.BaseValue)
            label.style.color = Color.green;
        if (stat.GetValue() < stat.BaseValue)
            label.style.color = Color.red;
    }

    public static List<VisualElement> HandleStatuses(CharacterStats stats)
    {
        List<VisualElement> els = new();
        if (stats.Statuses.Count == 0)
            return els;

        foreach (Status s in stats.Statuses)
        {
            VisualElement mElement = new VisualElement();
            mElement.style.backgroundImage = s.Icon.texture;
            mElement.AddToClassList("modifierIconContainer");
            els.Add(mElement);
        }
        return els;

    }

    public static List<VisualElement> HandleStatModifiers(CharacterStats stats)
    {
        List<VisualElement> els = new();
        foreach (Stat s in stats.Stats)
        {
            List<StatModifier> modifiers = s.GetActiveModifiers();
            if (modifiers.Count == 0)
                continue;

            foreach (StatModifier m in modifiers)
            {
                VisualElement mElement = new VisualElement();
                mElement.style.backgroundImage = m.Icon.texture;
                mElement.AddToClassList("modifierIconContainer");
                els.Add(mElement);
            }
        }
        return els;
    }

}

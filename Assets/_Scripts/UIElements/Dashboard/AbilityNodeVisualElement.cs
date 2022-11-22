using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityNodeVisualElement : VisualWithTooltip
{
    AbilityNode _abilityNode;

    VisualElement _tooltipElement;

    public AbilityNodeVisualElement(AbilityNode abilityNode)
    {
        _abilityNode = abilityNode;
        AddToClassList("abilityNodeContent");

        VisualElement icon = new();
        icon.AddToClassList("abilityNodeIcon");
        icon.style.backgroundImage = new StyleBackground(abilityNode.Icon);
        Add(icon);

        VisualElement cost = new();
        cost.style.flexDirection = FlexDirection.Row;
        cost.Add(new SpiceElement(abilityNode.AbilityNodeUnlockCost.YellowSpiceCost, SpiceColor.Yellow));
        cost.Add(new SpiceElement(abilityNode.AbilityNodeUnlockCost.BlueSpiceCost, SpiceColor.Blue));
        cost.Add(new SpiceElement(abilityNode.AbilityNodeUnlockCost.RedSpiceCost, SpiceColor.Red));
        Add(cost);

        _tooltipElement = new();
        _tooltipElement.AddToClassList("textPrimary");
        Label description = new(_abilityNode.Description);
        _tooltipElement.Add(description);
    }

    protected override void DisplayTooltip()
    {
        HideTooltip();

        _tooltip = new(this, _tooltipElement);

        base.DisplayTooltip();
    }


}

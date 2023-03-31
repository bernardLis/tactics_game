using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BattleTooltipElement : VisualElement
{
    Battle _battle;
    public BattleTooltipElement(Battle battle)
    {
        _battle = battle;
        style.flexDirection = FlexDirection.Row;

        foreach (ArmyGroup ag in _battle.Army)
        {
            ArmyElement el = new(ag);
            Add(el);
        }
    }
}

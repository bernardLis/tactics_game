using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class BattleTooltipElement : VisualElement
{
    Battle _battle;
    public BattleTooltipElement(Battle battle, Vector3 pos)
    {
        _battle = battle;
        style.flexDirection = FlexDirection.Row;

        foreach (ArmyGroup ag in _battle.Army)
        {
            ArmyElement el = new(ag);
            Add(el);
        }
        style.position = Position.Absolute;

        style.left = pos.x - 50;
        style.top = Screen.height - pos.y - 150;
    }
}

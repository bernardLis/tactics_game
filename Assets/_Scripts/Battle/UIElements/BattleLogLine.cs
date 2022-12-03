using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleLogLine : VisualElement
{
    public BattleLogLine(VisualElement element, BattleLogLineType type)
    {
        style.flexDirection = FlexDirection.Row;
        AddToClassList("battleLogLine");
        AddToClassList("textPrimary");

        Label icon = new();
        icon.style.width = 24;
        icon.style.height = 24;
        icon.style.backgroundImage = GameManager.Instance.GameDatabase.GetBattleLogLineIconByType(type).texture;
        Add(icon);
        Add(element);
    }
}

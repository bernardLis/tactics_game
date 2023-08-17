using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OpponentGroupCard : VisualElement
{
    public OpponentGroupCard(OpponentGroup opponentGroup)
    {
        style.backgroundColor = new Color(0, 0, 0, 0.5f);
        Add(new Label($"Number of minions: {opponentGroup.Minions.Count}"));
        Add(new Label($"Number of creatures: {opponentGroup.Creatures.Count}"));
    }
}

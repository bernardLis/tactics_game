using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JourneyNodeInfoVisual : VisualElement
{
    public JourneyNodeInfoVisual(JourneyNode node)
    {
        Label nodeType = new Label(node.NodeType.ToString());
        nodeType.AddToClassList("textPrimary");
        Add(nodeType);

    //    if (node.NodeType == JourneyNodeType.Battle)
           // HandleBattleNode((BattleNode)node);
    }

    void HandleBattleNode(Quest node)
    {
        Label mapInfo = new Label($"{node.MapVariant.name}, {node.Biome.name}, {node.MapSize.x}x{node.MapSize.y}");
        VisualElement enemyIconContainer = new VisualElement();
        enemyIconContainer.style.flexDirection = FlexDirection.Row;

        foreach (var e in node.Enemies)
        {
            Label l = new Label();
            l.style.backgroundImage = e.BrainIcon.texture;
            l.style.width = 32;
            l.style.height = 32;
            enemyIconContainer.Add(l);
        }

        mapInfo.AddToClassList("textPrimary");

        Add(mapInfo);
        Add(enemyIconContainer);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JourneyNodeInfoVisual : VisualElement
{
    public JourneyNodeInfoVisual(JourneyNode node)
    {
        Label nodeType = new Label(node.NodeType.ToString());
        nodeType.AddToClassList("primaryText");
        Add(nodeType);

        if (node.NodeType == JourneyNodeType.Battle)
            HandleBattleNode((BattleNode)node);
    }

    void HandleBattleNode(BattleNode node)
    {
        Label variant = new Label(node.MapVariant.name);
        Label biome = new Label(node.Biome.name);
        Label mapSize = new Label($"Map size: {node.MapSize.x} x {node.MapSize.y}");
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

        //Label numberOfEnemies = new Label("Number of enemies: " + .Count); // TODO: brains could hold icons that represent enemy type and I could be displaying them
        variant.AddToClassList("secondaryText");
        biome.AddToClassList("secondaryText");
        enemyIconContainer.AddToClassList("uiContainer");
        mapSize.AddToClassList("secondaryText");

        Add(variant);
        Add(biome);
        Add(enemyIconContainer);
        //Add(numberOfEnemies);
        Add(mapSize);
    }
}

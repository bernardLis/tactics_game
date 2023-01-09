using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestRankElement : VisualElement
{
    Quest _quest;
    public QuestRankElement(Quest quest)
    {
        _quest = quest;
        QuestRank questRank = _quest.Rank;
        style.alignItems = Align.Center;

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        Add(container);

        Label icon = new();
        icon.style.backgroundImage = new StyleBackground(questRank.Icon);
        icon.style.width = 48;
        icon.style.height = 48;
        container.Add(icon);

        ElementElement element = new(quest.ThreatElement);
        container.Add(element);

        Label rankTooltip = new($"{questRank.Description}");
        rankTooltip.style.whiteSpace = WhiteSpace.Normal;
        Add(new StarRankElement(questRank.Rank, 0.5f, rankTooltip));
    }

}

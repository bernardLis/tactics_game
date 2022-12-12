using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestRankVisualElement : VisualElement
{
    QuestRank _questRank;
    public QuestRankVisualElement(QuestRank questRank)
    {
        _questRank = questRank;
        style.alignItems = Align.Center;
        Label icon = new();
        icon.style.backgroundImage = new StyleBackground(questRank.Icon);
        icon.style.width = 48;
        icon.style.height = 48;

        Add(icon);

        Label rankTooltip = new($"{_questRank.Description}");
        rankTooltip.style.whiteSpace = WhiteSpace.Normal;
        Add(new StarRankVisualElement(_questRank.Rank, 0.5f, rankTooltip));
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestRankElement : VisualElement
{
    Quest _quest;
    VisualElement _elementalElementContainer;
    public QuestRankElement(Quest quest)
    {
        _quest = quest;
        QuestRank questRank = _quest.Rank;
        style.alignItems = Align.Center;

        _elementalElementContainer = new();
        _elementalElementContainer.style.flexDirection = FlexDirection.Row;
        Add(_elementalElementContainer);

        Label rankTooltip = new($"{questRank.Description}");
        rankTooltip.style.whiteSpace = WhiteSpace.Normal;
        Add(new StarRankElement(questRank.Rank, 0.5f, rankTooltip));
    }

    public void UpdateElementalElement(int infoRank)
    {
        _elementalElementContainer.Clear();

        Label icon = new();
        icon.style.backgroundImage = new StyleBackground(_quest.Rank.Icon);
        icon.style.width = 48;
        icon.style.height = 48;
        _elementalElementContainer.Add(icon);

        if (infoRank >= 2)
        {
            ElementalElement element = new(_quest.ThreatElement);
            _elementalElementContainer.Add(element);
        }
        else
        {
            TextWithTooltip tt = new("??", "Element unknown windmill spy not upgraded");
            _elementalElementContainer.Add(tt);
        }
    }
}

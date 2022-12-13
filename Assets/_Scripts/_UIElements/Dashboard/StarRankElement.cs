using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StarRankElement : ElementWithTooltip
{
    List<VisualElement> stars = new();
    int _rank;
    VisualElement _tooltipElement;

    public StarRankElement(int rank, float scale = 1f, VisualElement tooltip = null)
    {
        style.flexDirection = FlexDirection.Row;
        transform.scale = Vector3.one * scale;

        _rank = rank;
        for (int i = 0; i < 5; i++)
        {
            VisualElement star = new();
            star.AddToClassList("rankStarGray");
            Add(star);
            stars.Add(star);
        }
        SetRank(rank);

        if (tooltip != null)
            _tooltipElement = tooltip;
    }

    public void AddRank()
    {
        _rank++;
        SetRank(_rank);
    }

    public void SubtractRank()
    {
        _rank--;
        SetRank(_rank);
    }

    public void SetRank(int rank)
    {
        for (int i = 0; i < stars.Count; i++)
        {
            VisualElement star = stars[i];
            star.RemoveFromClassList("rankStar");
            if (i < rank)
                star.AddToClassList("rankStar");
        }
    }

    protected override void DisplayTooltip()
    {
        if (_tooltipElement != null)
        {
            _tooltip = new(this, _tooltipElement);
        }
        else
        {
            Label l = new Label("rank tooltip to be implemented");
            l.style.whiteSpace = WhiteSpace.Normal;
            _tooltip = new(this, l);

        }

        base.DisplayTooltip();
    }

}

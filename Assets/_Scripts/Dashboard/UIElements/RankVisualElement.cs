using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RankVisualElement : VisualWithTooltip
{
    List<VisualElement> stars = new();
    int _rank;

    public RankVisualElement(int rank, float scale = 1f)
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
        _tooltip = new(this, new Label("rank tooltip to be implemented"));
        base.DisplayTooltip();
    }

}

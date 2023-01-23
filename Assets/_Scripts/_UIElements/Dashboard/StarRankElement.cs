using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class StarRankElement : ElementWithTooltip
{
    List<VisualElement> stars = new();
    int _rank;
    VisualElement _tooltipElement;

    const string _ussClassName = "star-rank-element";
    const string _ussStar = _ussClassName + "__star";
    const string _ussStarGold = _ussClassName + "__star-gold";
    const string _ussStarGray = _ussClassName + "__star-gray";

    const string _ussEffect = _ussClassName + "__effect";


    public StarRankElement(int rank, float scale = 1f, VisualElement tooltip = null, int maxRank = 5)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StarRankElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        style.flexDirection = FlexDirection.Row;
        transform.scale = Vector3.one * scale;

        _rank = rank;
        for (int i = 0; i < maxRank; i++)
        {
            VisualElement star = new();
            star.AddToClassList(_ussStar);
            star.AddToClassList(_ussStarGray);
            Add(star);
            stars.Add(star);
        }
        SetRank(rank);

        if (tooltip != null)
            _tooltipElement = tooltip;
    }

    public async void SetRank(int rank)
    {
        for (int i = 0; i < stars.Count; i++)
        {
            VisualElement star = stars[i];
            star.AddToClassList(_ussStarGray);
            star.RemoveFromClassList(_ussStarGold);
            star.AddToClassList(_ussEffect);
            if (i < rank)
            {
                star.RemoveFromClassList(_ussStarGray);
                star.AddToClassList(_ussStarGold);
            }
            await Task.Delay(100);
            star.RemoveFromClassList(_ussEffect);
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
            Label l = new Label("Rank tooltip to be implemented");
            l.style.whiteSpace = WhiteSpace.Normal;
            _tooltip = new(this, l);
        }

        base.DisplayTooltip();
    }

}

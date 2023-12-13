using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TabletElement : ElementWithTooltip
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonButtonBasic = "common__button-basic";

    const string _ussClassName = "tablet-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIcon = _ussClassName + "icon";
    const string _ussLevelDotEmpty = _ussClassName + "level-dot-empty";
    const string _ussLevelDotFull = _ussClassName + "level-dot-full";

    VisualElement _icon;

    public Tablet Tablet;

    public TabletElement(Tablet tablet, bool showLevel = false) : base()
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.TabletElementStyles);
        if (ss != null) styleSheets.Add(ss);

        Tablet = tablet;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussCommonButtonBasic);

        _icon = new();
        _icon.AddToClassList(_ussIcon);
        _icon.style.backgroundImage = tablet.Icon.texture;
        Add(_icon);

        if (showLevel) AddLevelUpDots();
    }

    void AddLevelUpDots()
    {
        VisualElement dotContainer = new();
        dotContainer.style.flexDirection = FlexDirection.Row;
        dotContainer.style.position = Position.Absolute;
        dotContainer.style.top = Length.Percent(15);
        Add(dotContainer);
        List<VisualElement> dots = new();
        for (int i = 0; i < Tablet.MaxLevel; i++)
        {
            VisualElement dot = new();
            dot.AddToClassList(_ussLevelDotEmpty);
            dots.Add(dot);
            dotContainer.Add(dot);
        }

        for (int i = 0; i < Tablet.Level.Value; i++)
            dots[i].AddToClassList(_ussLevelDotFull);

        Tablet.OnLevelUp += () =>
        {
            for (int i = 0; i < Tablet.Level.Value; i++)
                dots[i].AddToClassList(_ussLevelDotFull);
        };
    }

    protected override void DisplayTooltip()
    {
        VisualElement tooltip = new();
        // HERE: tooltip
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }
}

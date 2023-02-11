using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WagesReportElement : ReportElement
{
    public WagesReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        AddHeader("Wages paid", Color.yellow);

        ScrollView scrollView = new();
        _reportContents.Add(scrollView);
        foreach (Character c in report.Characters)
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            CharacterPortraitElement portraitElement = new(c);
            Label dots = new(".........................");
            GoldElement wageGoldElement = new(c.WeeklyWage.Value);

            container.Add(portraitElement);
            container.Add(dots);
            container.Add(wageGoldElement);

            scrollView.Add(container);
        }

        AddSignButton();
        ShowSignButton();
    }

}

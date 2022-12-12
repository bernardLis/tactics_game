using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TextReportVisualElement : ReportVisualElement
{
    public TextReportVisualElement(VisualElement parent, Report report) : base(parent, report)
    {
        AddHeader("New Message", new Color(0.27f, 0.56f, 0.34f));

        Label text = new(_report.Text);
        text.style.width = Length.Percent(70f);
        text.style.fontSize = 36;
        text.style.whiteSpace = WhiteSpace.Normal;
        _reportContents.Add(text);

        AddSignButton();
        ShowSignButton();
    }


}

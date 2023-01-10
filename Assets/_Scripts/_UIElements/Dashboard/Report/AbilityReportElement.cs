using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityReportElement : ReportElement
{
    public AbilityReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.AbilityReportStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddHeader("New Ability", Color.cyan);

        AbilitySlot slot = new(new AbilityButton(report.Ability));
        _reportContents.Add(slot);

        AddSignButton();
        ShowSignButton();
    }


}

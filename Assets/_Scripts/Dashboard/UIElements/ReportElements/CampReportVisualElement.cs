using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampReportVisualElement : ReportVisualElement
{
    public CampReportVisualElement(VisualElement parent, Report report) : base(parent, report)
    {
        CampBuilding b = _gameManager.GetCampBuildingById(_report.CampBuildingId);
        AddHeader("Building Finished!", new Color(0.66f, 0.42f, 0.17f));

        CampBuildingVisualElement el = new CampBuildingVisualElement(b);
        el.style.backgroundImage = null; // looks weird coz report already has paper bg
        _reportContents.Add(el);

        AddSignButton();
        ShowSignButton();
    }
}

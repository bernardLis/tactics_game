using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampReportElement : ReportElement
{
    public CampReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        CampBuilding b = _gameManager.GetComponent<BuildingManager>().GetCampBuildingById(_report.CampBuildingId);
        AddHeader("Building Finished!", new Color(0.66f, 0.42f, 0.17f));

        CampBuildingElement el = null;
        if (b.GetType().Equals(typeof(CampBuildingTroopsLimit)))
            el = (CampBuildingTroopsLimitElement)new CampBuildingTroopsLimitElement((CampBuildingTroopsLimit)b);
        if (b.GetType().Equals(typeof(CampBuildingQuests)))
            el = (CampBuildingQuestsElement)new CampBuildingQuestsElement((CampBuildingQuests)b);
        if (b.GetType().Equals(typeof(CampBuildingPawnshop)))
            el = (CampBuildingPawnshopElement)new CampBuildingPawnshopElement((CampBuildingPawnshop)b);
        if (b.GetType().Equals(typeof(CampBuildingSpiceRecycler)))
            el = (CampBuildingSpiceRecyclerElement)new CampBuildingSpiceRecyclerElement((CampBuildingSpiceRecycler)b);
        if (b.GetType().Equals(typeof(CampBuildingShop)))
            el = (CampBuildingShopElement)new CampBuildingShopElement((CampBuildingShop)b);
        if (b.GetType().Equals(typeof(CampBuildingRecruiting)))
            el = (CampBuildingRecruitingElement)new CampBuildingRecruitingElement((CampBuildingRecruiting)b);
        if (b.GetType().Equals(typeof(CampBuildingHospital)))
            el = (CampBuildingHospitalElement)new CampBuildingHospitalElement((CampBuildingHospital)b);
        if (b.GetType().Equals(typeof(CampBuildingGoldProduction)))
            el = (CampBuildingGoldProductionElement)new CampBuildingGoldProductionElement((CampBuildingGoldProduction)b);
        if (b.GetType().Equals(typeof(CampBuildingSpiceProduction)))
            el = (CampBuildingSpiceProductionElement)new CampBuildingSpiceProductionElement((CampBuildingSpiceProduction)b);

        el.style.backgroundImage = null; // looks weird coz report already has paper bg
        _reportContents.Add(el);

        AddSignButton();
        ShowSignButton();
    }
}

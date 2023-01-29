using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;

    VisualElement _root;
    VisualElement _mainCamp;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += DayPassed;

        _dashboardManager = GetComponent<DashboardManager>();
        _root = _dashboardManager.Root;
        _mainCamp = _root.Q<VisualElement>("mainCamp");

        Initialize();
    }

    void Initialize()
    {
        _mainCamp.Clear();
        List<CampBuilding> buildings = new(_gameManager.GetComponent<BuildingManager>().GetAllCampBuildings());
        foreach (CampBuilding b in buildings)
        {
            if (b.GetType().Equals(typeof(CampBuildingTroopsLimit)))
                _mainCamp.Add(new CampBuildingTroopsLimitElement((CampBuildingTroopsLimit)b));
            if (b.GetType().Equals(typeof(CampBuildingQuests)))
                _mainCamp.Add(new CampBuildingQuestsElement((CampBuildingQuests)b));
            if (b.GetType().Equals(typeof(CampBuildingPawnshop)))
                _mainCamp.Add(new CampBuildingPawnshopElement((CampBuildingPawnshop)b));
            if (b.GetType().Equals(typeof(CampBuildingSpiceRecycler)))
                _mainCamp.Add(new CampBuildingSpiceRecyclerElement((CampBuildingSpiceRecycler)b));
            if (b.GetType().Equals(typeof(CampBuildingShop)))
                _mainCamp.Add(new CampBuildingShopElement((CampBuildingShop)b));
            if (b.GetType().Equals(typeof(CampBuildingRecruiting)))
                _mainCamp.Add(new CampBuildingRecruitingElement((CampBuildingRecruiting)b));
            if (b.GetType().Equals(typeof(CampBuildingHospital)))
                _mainCamp.Add(new CampBuildingHospitalElement((CampBuildingHospital)b));
            if (b.GetType().Equals(typeof(CampBuildingGoldProduction)))
                _mainCamp.Add(new CampBuildingGoldProductionElement((CampBuildingGoldProduction)b));
            if (b.GetType().Equals(typeof(CampBuildingSpiceProduction)))
                _mainCamp.Add(new CampBuildingSpiceProductionElement((CampBuildingSpiceProduction)b));
            if (b.GetType().Equals(typeof(CampBuildingItemProduction)))
                _mainCamp.Add(new CampBuildingItemProductionElement((CampBuildingItemProduction)b));
            if (b.GetType().Equals(typeof(CampBuildingNegotiation)))
                _mainCamp.Add(new CampBuildingNegotiationElement((CampBuildingNegotiation)b));
            if (b.GetType().Equals(typeof(CampBuildingQuestInfo)))
                _mainCamp.Add(new CampBuildingQuestInfoElement((CampBuildingQuestInfo)b));
        }
    }

    void DayPassed(int day)
    {

    }

}

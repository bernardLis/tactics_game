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
        List<CampBuilding> buildings = new(_gameManager.GetCampBuildings());
        foreach (CampBuilding b in buildings)
        _mainCamp.Add(new CampBuildingElement(b));
    }

    void DayPassed(int day)
    {

    }

}

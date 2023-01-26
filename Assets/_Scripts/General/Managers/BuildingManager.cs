using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    GameManager _gameManager;

    CampBuildingPawnshop _pawnshopBuilding;
    CampBuildingSpiceRecycler _spiceRecyclerBuilding;
    CampBuildingShop _shopBuilding;
    void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _gameManager.OnDayPassed += OnDayPassed;

        foreach (CampBuilding cb in _gameManager.GetCampBuildings())
        {
            if (cb.GetType().Equals(typeof(CampBuildingPawnshop)))
                _pawnshopBuilding = (CampBuildingPawnshop)cb;
            if (cb.GetType().Equals(typeof(CampBuildingSpiceRecycler)))
                _spiceRecyclerBuilding = (CampBuildingSpiceRecycler)cb;
            if (cb.GetType().Equals(typeof(CampBuildingShop)))
                _shopBuilding = (CampBuildingShop)cb;

        }
    }

    void OnDayPassed(int day)
    {
        if (Random.value < 0.5f)
            AddRandomQuest();
        if (Random.value < 0.5f)
            AddRecruit();
        if (Random.value < 0.5f)
            AddShop();
        if (Random.value < _pawnshopBuilding.GetVisitChance())
            AddPawnshop();
        if (Random.value < _spiceRecyclerBuilding.GetVisitChance())
            AddSpiceRecycle();
    }

    public void AddRandomQuest()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        Quest q = ScriptableObject.CreateInstance<Quest>();
        q.CreateRandom();
        _gameManager.OnDayPassed += q.OnDayPassed;

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Quest, q);
        _gameManager.AddNewReport(r);
    }

    void AddRecruit()
    {
        Recruit newRecruit = ScriptableObject.CreateInstance<Recruit>();
        newRecruit.CreateRandom();

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Recruit, null, newRecruit);
        _gameManager.AddNewReport(r);
    }

    void AddShop()
    {
        Shop newShop = ScriptableObject.CreateInstance<Shop>();
        newShop.CreateShop(_shopBuilding);

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Shop, null, null, null, null, newShop);
        _gameManager.AddNewReport(r);
    }

    void AddPawnshop()
    {
        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Pawnshop, null, null, null, null, null);
        _gameManager.AddNewReport(r);
    }

    void AddSpiceRecycle()
    {
        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.SpiceRecycle, null, null, null, null, null);
        _gameManager.AddNewReport(r);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    GameManager _gameManager;

    CampBuildingPawnshop _pawnshopBuilding;
    CampBuildingSpiceRecycler _spiceRecyclerBuilding;
    CampBuildingShop _shopBuilding;
    CampBuildingRecruiting _recruitingBuilding;

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
            if (cb.GetType().Equals(typeof(CampBuildingRecruiting)))
                _recruitingBuilding = (CampBuildingRecruiting)cb;

        }
    }

    void OnDayPassed(int day)
    {
        if (Random.value < 0.5f)
            AddRandomQuest();

        if (Random.value < _pawnshopBuilding.GetUpgradeByRank(_pawnshopBuilding.UpgradeRank).ChanceToVisit)
            AddPawnshop();
        if (Random.value < _spiceRecyclerBuilding.GetUpgradeByRank(_pawnshopBuilding.UpgradeRank).ChanceToVisit)
            AddSpiceRecycle();

        if (_shopBuilding.CampBuildingState == CampBuildingState.Built
                && Random.value < 0.3f)
            AddShop();

        if (_recruitingBuilding.CampBuildingState == CampBuildingState.Built
                && Random.value < 0.3f)
            AddRecruit();
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
        int level = Random.Range(1, _recruitingBuilding.GetUpgradeByRank(_recruitingBuilding.UpgradeRank).MaxRecruitLevel + 1);
        newRecruit.CreateRandom(level);

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

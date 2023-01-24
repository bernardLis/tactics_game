using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    GameManager _gameManager;

    void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _gameManager.OnDayPassed += OnDayPassed;

        foreach (CampBuilding cb in _gameManager.GetCampBuildings())
        {
            if (cb.GetType().Equals(typeof(CampBuildingBetterQuests)))
                Debug.Log($"heyo");


        }
    }

    void OnDayPassed(int day)
    {
        if (Random.value > 0.5f)
            AddRandomQuest();
        if (Random.value > 0.5f)
            AddRecruit();
        if (Random.value > 0.5f)
            AddShop();
            
        AddPawnshop();
        AddSpiceRecycle();
    }

    public void AddRandomQuest()
    {
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
        newShop.CreateShop();

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] List<CampBuilding> _campBuildings = new();
    public List<CampBuilding> GetAllCampBuildings() { return _campBuildings; }
    public CampBuilding GetCampBuildingById(string id) { return _campBuildings.FirstOrDefault(x => x.Id == id); }

    public CampBuildingTroopsLimit TroopsLimitBuilding { get; private set; }
    public CampBuildingQuests QuestsBuilding { get; private set; }
    public CampBuildingHospital HospitalBuilding { get; private set; }
    public CampBuildingGoldProduction CampBuildingGoldProduction { get; private set; }
    public CampBuildingSpiceProduction CampBuildingSpiceProduction { get; private set; }
    public CampBuildingItemProduction CampBuildingItemProduction { get; private set; }

    CampBuildingPawnshop _pawnshopBuilding;
    CampBuildingSpiceRecycler _spiceRecyclerBuilding;
    CampBuildingShop _shopBuilding;
    CampBuildingRecruiting _recruitingBuilding;

    void Awake()
    {
        foreach (CampBuilding cb in _campBuildings)
        {
            if (cb.GetType().Equals(typeof(CampBuildingTroopsLimit)))
                TroopsLimitBuilding = (CampBuildingTroopsLimit)cb;
            if (cb.GetType().Equals(typeof(CampBuildingQuests)))
                QuestsBuilding = (CampBuildingQuests)cb;
            if (cb.GetType().Equals(typeof(CampBuildingHospital)))
                HospitalBuilding = (CampBuildingHospital)cb;
            if (cb.GetType().Equals(typeof(CampBuildingPawnshop)))
                _pawnshopBuilding = (CampBuildingPawnshop)cb;
            if (cb.GetType().Equals(typeof(CampBuildingSpiceRecycler)))
                _spiceRecyclerBuilding = (CampBuildingSpiceRecycler)cb;
            if (cb.GetType().Equals(typeof(CampBuildingShop)))
                _shopBuilding = (CampBuildingShop)cb;
            if (cb.GetType().Equals(typeof(CampBuildingRecruiting)))
                _recruitingBuilding = (CampBuildingRecruiting)cb;
            if (cb.GetType().Equals(typeof(CampBuildingGoldProduction)))
                CampBuildingGoldProduction = (CampBuildingGoldProduction)cb;
            if (cb.GetType().Equals(typeof(CampBuildingSpiceProduction)))
                CampBuildingSpiceProduction = (CampBuildingSpiceProduction)cb;
            if (cb.GetType().Equals(typeof(CampBuildingItemProduction)))
                CampBuildingItemProduction = (CampBuildingItemProduction)cb;
        }

        _gameManager = GetComponent<GameManager>();
        _gameManager.OnNewSaveFileCreation += OnNewSaveFileCreation;
        _gameManager.OnClearSaveData += OnClearSaveData;
        _gameManager.OnDayPassed += OnDayPassed;
    }

    void OnNewSaveFileCreation()
    {
        foreach (CampBuilding b in _campBuildings)
        {
            b.ResetSelf();
            b.Initialize();
        }

        // TODO: // HERE: for now, I could hand craft 3 first quests or something...
        for (int i = 0; i < 3; i++)
            AddRandomQuest();
    }

    void OnClearSaveData()
    {
        foreach (CampBuilding b in _campBuildings)
            b.ResetSelf();
    }

    public void LoadAllBuildingsFromData(List<CampBuildingData> datas)
    {
        foreach (CampBuildingData d in datas)
            GetCampBuildingById(d.Id).LoadFromData(d);
    }

    void OnDayPassed(int day)
    {
        CampBuildingItemProduction.Produce();
        CampBuildingGoldProduction.Produce();
        CampBuildingSpiceProduction.Produce();

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

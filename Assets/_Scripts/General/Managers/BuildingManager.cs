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
    public CampBuildingGoldProduction GoldProductionBuilding { get; private set; }
    public CampBuildingSpiceProduction SpiceProductionBuilding { get; private set; }
    public CampBuildingItemProduction ItemProductionBuilding { get; private set; }
    public CampBuildingNegotiation NegotiationBuilding { get; private set; }
    public CampBuildingQuestInfo QuestInfoBuilding { get; private set; }

    CampBuildingPawnshop _pawnshopBuilding;
    CampBuildingSpiceRecycler _spiceRecyclerBuilding;
    public CampBuildingShop ShopBuilding { get; private set; }
    CampBuildingRecruiting _recruitingBuilding;

    void Awake()
    {
        foreach (CampBuilding cb in _campBuildings)
        {
            cb.Initialize();
            
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
                ShopBuilding = (CampBuildingShop)cb;
            if (cb.GetType().Equals(typeof(CampBuildingRecruiting)))
                _recruitingBuilding = (CampBuildingRecruiting)cb;
            if (cb.GetType().Equals(typeof(CampBuildingGoldProduction)))
                GoldProductionBuilding = (CampBuildingGoldProduction)cb;
            if (cb.GetType().Equals(typeof(CampBuildingSpiceProduction)))
                SpiceProductionBuilding = (CampBuildingSpiceProduction)cb;
            if (cb.GetType().Equals(typeof(CampBuildingItemProduction)))
                ItemProductionBuilding = (CampBuildingItemProduction)cb;
            if (cb.GetType().Equals(typeof(CampBuildingNegotiation)))
                NegotiationBuilding = (CampBuildingNegotiation)cb;
            if (cb.GetType().Equals(typeof(CampBuildingQuestInfo)))
                QuestInfoBuilding = (CampBuildingQuestInfo)cb;
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
        ItemProductionBuilding.Produce();
        GoldProductionBuilding.Produce();
        SpiceProductionBuilding.Produce();

        if (Random.value < 0.5f)
            AddRandomQuest();

        if (Random.value < _pawnshopBuilding.GetUpgradeByRank(_pawnshopBuilding.UpgradeRank).ChanceToVisit)
            AddPawnshop();
        if (Random.value < _spiceRecyclerBuilding.GetUpgradeByRank(_pawnshopBuilding.UpgradeRank).ChanceToVisit)
            AddSpiceRecycle();

        // if (ShopBuilding.CampBuildingState == CampBuildingState.Built
        //           && Random.value < 0.3f)
        AddShop();

        if (_recruitingBuilding.CampBuildingState == CampBuildingState.Built
                   && Random.value < 0.3f)
            AddRecruit();
    }

    public void AddRandomQuest()
    {
        Debug.Log($"Adding a random quest.");

        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        Quest q = ScriptableObject.CreateInstance<Quest>();
        q.CreateRandom();

        DateTime expiry = ScriptableObject.CreateInstance<DateTime>();
        expiry.Day = _gameManager.Day + Random.Range(2, 5);

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Quest, q, expiryDateTime: expiry);
        _gameManager.AddNewReport(r);
    }

    public void AddRecruit()
    {
        Debug.Log($"Adding new recruit.");

        Recruit newRecruit = ScriptableObject.CreateInstance<Recruit>();
        int level = Random.Range(1, _recruitingBuilding.GetUpgradeByRank(_recruitingBuilding.UpgradeRank).MaxRecruitLevel + 1);
        newRecruit.CreateRandom(level);

        DateTime expiry = ScriptableObject.CreateInstance<DateTime>();
        expiry.Day = _gameManager.Day + Random.Range(1, 3);

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Recruit, recruit: newRecruit, expiryDateTime: expiry);
        _gameManager.AddNewReport(r);
    }

    public void AddShop()
    {
        Debug.Log($"Adding a shop.");
        Shop newShop = ScriptableObject.CreateInstance<Shop>();
        newShop.CreateShop(ShopBuilding);

        DateTime expiry = ScriptableObject.CreateInstance<DateTime>();
        expiry.Day = _gameManager.Day + Random.Range(1, 3);

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Shop, shop: newShop, expiryDateTime: expiry);
        _gameManager.AddNewReport(r);
    }

    public void AddPawnshop()
    {
        Debug.Log($"Adding a pawnshop.");

        DateTime expiry = ScriptableObject.CreateInstance<DateTime>();
        expiry.Day = _gameManager.Day + Random.Range(1, 3);

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Pawnshop, expiryDateTime: expiry);
        _gameManager.AddNewReport(r);
    }

    public void AddSpiceRecycle()
    {
        Debug.Log($"Adding spice recycler.");

        DateTime expiry = ScriptableObject.CreateInstance<DateTime>();
        expiry.Day = _gameManager.Day + Random.Range(1, 3);

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.SpiceRecycle, expiryDateTime: expiry);
        _gameManager.AddNewReport(r);
    }
}

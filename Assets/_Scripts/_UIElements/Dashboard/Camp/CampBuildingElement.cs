using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingElement : VisualElement
{
    protected GameManager _gameManager;

    protected CampBuilding _campBuilding;

    VisualElement _sprite;
    Label _timeToBuild;
    VisualElement _buildingRankContainer;
    StarRankElement _buildingRankElement;

    VisualElement _buildButtonContainer;
    MyButton _buildButton;
    VisualElement _upgradeCostContainer;
    GoldElement _costGoldElement;

    protected VisualElement _upgradeContainer;
    TextWithTooltip _upgradeText;

    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonTextPrimaryBlack = "common__text-primary-black";

    const string _ussClassName = "camp-building__";
    const string _ussMain = _ussClassName + "main";
    const string _ussSprite = _ussClassName + "sprite";
    const string _ussUpgradeCostContainer = _ussClassName + "upgrade-cost-container";
    const string _ussBuildButton = _ussClassName + "build-button";

    public CampBuildingElement(CampBuilding campBuilding)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CampBuildingStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _gameManager.OnDayPassed += OnDayPassed;
        _gameManager.OnGoldChanged += OnGoldChanged;

        _campBuilding = campBuilding;
        _campBuilding.OnCampBuildingStateChanged += OnCampBuildingStateChanged;
        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        Label header = new($"{_campBuilding.DisplayName}");
        header.style.fontSize = 36;
        Add(header);

        HandleUpgradeReward();

        _sprite = new();
        _sprite.AddToClassList(_ussSprite);
        Add(_sprite);
        UpdateBuildingSprite();

        _buildingRankContainer = new();
        Add(_buildingRankContainer);
        HandleBuildingRank();

        _buildButtonContainer = new();
        Add(_buildButtonContainer);
        UpdateBuildButton();
    }

    void OnDayPassed(int day) { UpdateBuildButton(); }

    void OnGoldChanged(int gold) { UpdateBuildButton(); }

    void OnCampBuildingStateChanged(CampBuildingState newState)
    {
        if (newState == CampBuildingState.Built)
            BuildingFinished();
    }

    void UpdateBuildingSprite()
    {
        _sprite.style.backgroundImage = new StyleBackground(_campBuilding.OutlineSprite);

        if (_campBuilding.CampBuildingState == CampBuildingState.Built)
            _sprite.style.backgroundImage = new StyleBackground(_campBuilding.BuiltSprite);
    }

    void HandleUpgradeReward()
    {
        _upgradeContainer = new();
        _upgradeContainer.style.flexDirection = FlexDirection.Row;
        AddUpgrade();
        SetUpgrade();

        if (_campBuilding.GetType().Equals(typeof(CampBuildingShop)))
            AddShopUpgrade();

        if (_campBuilding.GetType().Equals(typeof(CampBuildingRecruiting)))
            AddRecruitingUpgrade();

        if (_campBuilding.GetType().Equals(typeof(CampBuildingHospital)))
            AddHospitalUpgrade();

        Add(_upgradeContainer);
    }

    protected virtual void AddUpgrade()
    {
        // meant to be overwritten
    }

    protected virtual void SetUpgrade()
    {
        // meant to be overwritten
    }


    void AddShopUpgrade()
    {
        string tooltipText = "Chance for uncommon, rare and epic item in the shop.";
        _upgradeText = new("TXT", tooltipText);
        SetShopUpgrade();
        _upgradeText.UpdateFontSize(18);
        _upgradeContainer.Add(_upgradeText);
    }

    void SetShopUpgrade()
    {
        CampBuildingShop c = (CampBuildingShop)_campBuilding;
        CampUpgradeShop upgrade = c.GetUpgradeByRank(c.UpgradeRank);
        string labelText = $"{upgrade.UncommonItemChance}, {upgrade.RareItemChance}, {upgrade.EpicItemChance}";
        _upgradeText.UpdateText(labelText);
        _upgradeText.UpdateTextColor(Color.white);
    }

    void AddRecruitingUpgrade()
    {
        string tooltipText = "Max new recruit level.";
        _upgradeText = new("TXT", tooltipText);
        SetRecruitingUpgrade();
        _upgradeText.UpdateFontSize(36);
        _upgradeContainer.Add(_upgradeText);
    }

    void SetRecruitingUpgrade()
    {
        CampBuildingRecruiting c = (CampBuildingRecruiting)_campBuilding;
        CampUpgradeRecruiting upgrade = c.GetUpgradeByRank(c.UpgradeRank);
        string labelText = $"{upgrade.MaxRecruitLevel}";
        _upgradeText.UpdateText(labelText);
        _upgradeText.UpdateTextColor(Color.white);
    }

    void AddHospitalUpgrade()
    {
        string tooltipText = "Max days disabled on quest fail.";
        _upgradeText = new("TXT", tooltipText);
        SetHospitalUpgrade();
        _upgradeText.UpdateFontSize(36);
        _upgradeContainer.Add(_upgradeText);
    }

    void SetHospitalUpgrade()
    {
        CampBuildingHospital c = (CampBuildingHospital)_campBuilding;
        CampUpgradeHospital upgrade = c.GetUpgradeByRank(c.UpgradeRank);
        string labelText = $"{upgrade.MaxDaysDisabled}";
        _upgradeText.UpdateText(labelText);
        _upgradeText.UpdateTextColor(Color.white);
    }

    void HandleBuildingRank()
    {
        _buildingRankElement = new(_campBuilding.UpgradeRank, 0.7f, null, _campBuilding.GetMaxUpgradeRank());
        _buildingRankContainer.Add(_buildingRankElement);
    }

    void UpdateBuildButton()
    {
        _buildButtonContainer.Clear();

        if (_campBuilding.CampBuildingState == CampBuildingState.NotBuilt)
            HandleBuildButtonNotBuilt();

        if (_campBuilding.CampBuildingState == CampBuildingState.Building)
            HandleBuildButtonBuilding();

        if (_campBuilding.CampBuildingState == CampBuildingState.Built)
            HandleBuildButtonUpgrade();
    }

    void HandleBuildButtonNotBuilt()
    {
        _buildButton = new(null, _ussBuildButton, Build);
        _buildButton.RegisterCallback<PointerEnterEvent>(BuildButtonPointerEnter);
        _buildButton.RegisterCallback<PointerLeaveEvent>(BuildButtonPointerLeave);
        _buildButton.RegisterCallback<PointerUpEvent>(BuildButtonPointerUp);
        _costGoldElement = new GoldElement(_campBuilding.CostToBuild);
        _buildButton.Add(_costGoldElement);
        _buildButton.SetEnabled(false);

        _buildButtonContainer.Add(_buildButton);
        _timeToBuild = new($"Time to build: {_campBuilding.DaysToBuild} days");
        _buildButtonContainer.Add(_timeToBuild);

        if (_gameManager.Gold >= _campBuilding.CostToBuild)
            _buildButton.SetEnabled(true);
    }

    void HandleBuildButtonBuilding()
    {
        _buildButton = new(null, _ussBuildButton, Build);
        _buildButtonContainer.Add(_buildButton);
        _buildButton.SetEnabled(false);

        _buildButton.SetEnabled(false);
        _timeToBuild = new($"Days left: {_campBuilding.DaysLeftToBuild}");
        _buildButton.Add(_timeToBuild);
        return;
    }

    void HandleBuildButtonUpgrade()
    {
        if (_campBuilding.UpgradeRank >= _campBuilding.GetMaxUpgradeRank())
            return;

        _buildButton = new(null, _ussBuildButton, Upgrade);
        _buildButton.RegisterCallback<PointerEnterEvent>(BuildButtonPointerEnter);
        _buildButton.RegisterCallback<PointerLeaveEvent>(BuildButtonPointerLeave);
        _buildButton.SetEnabled(false);

        _costGoldElement = new GoldElement(_campBuilding.GetUpgradeCost());
        _buildButton.Add(_costGoldElement);

        _buildButtonContainer.Add(_buildButton);

        if (_gameManager.Gold >= _campBuilding.GetUpgradeCost())
            _buildButton.SetEnabled(true);
    }

    void Upgrade()
    {
        _campBuilding.Upgrade();
        UpdateBuildButton();
        _buildingRankElement.SetRank(_campBuilding.UpgradeRank);
    }

    protected virtual void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        if (_campBuilding.GetType().Equals(typeof(CampBuildingShop)))
        {
            CampBuildingShop c = (CampBuildingShop)_campBuilding;
            CampUpgradeShop upgrade = c.GetUpgradeByRank(c.UpgradeRank + 1);
            string labelText = $"{upgrade.UncommonItemChance}, {upgrade.RareItemChance}, {upgrade.EpicItemChance}";
            _upgradeText.UpdateText(labelText);
            _upgradeText.UpdateTextColor(Color.green);
        }

        if (_campBuilding.GetType().Equals(typeof(CampBuildingRecruiting)))
        {
            CampBuildingRecruiting c = (CampBuildingRecruiting)_campBuilding;
            CampUpgradeRecruiting upgrade = c.GetUpgradeByRank(c.UpgradeRank + 1);
            string labelText = $"{upgrade.MaxRecruitLevel}";
            _upgradeText.UpdateText(labelText);
            _upgradeText.UpdateTextColor(Color.green);
        }

        if (_campBuilding.GetType().Equals(typeof(CampBuildingHospital)))
        {
            CampBuildingHospital c = (CampBuildingHospital)_campBuilding;
            CampUpgradeHospital upgrade = c.GetUpgradeByRank(c.UpgradeRank + 1);
            string labelText = $"{upgrade.MaxDaysDisabled}";
            _upgradeText.UpdateText(labelText);
            _upgradeText.UpdateTextColor(Color.green);
        }

    }

    void BuildButtonPointerLeave(PointerLeaveEvent evt) { ResetUpgradeContainer(); }

    void BuildButtonPointerUp(PointerUpEvent evt) { ResetUpgradeContainer(); }

    void ResetUpgradeContainer()
    {
        SetUpgrade();

        if (_campBuilding.GetType().Equals(typeof(CampBuildingShop)))
            SetShopUpgrade();

        if (_campBuilding.GetType().Equals(typeof(CampBuildingRecruiting)))
            SetRecruitingUpgrade();

        if (_campBuilding.GetType().Equals(typeof(CampBuildingHospital)))
            SetHospitalUpgrade();
    }

    void Build()
    {
        _campBuilding.StartBuilding();
        _gameManager.ChangeGoldValue(-_campBuilding.CostToBuild);
        UpdateBuildButton();
        _costGoldElement.ChangeAmount(0);
    }

    void BuildingFinished()
    {
        UpdateBuildingSprite();
        UpdateBuildButton();
        ResetUpgradeContainer();

        _buildingRankElement.SetRank(_campBuilding.UpgradeRank);
        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.CampBuilding, null, null, null, _campBuilding.Id);
        _gameManager.AddNewReport(r);
    }
}

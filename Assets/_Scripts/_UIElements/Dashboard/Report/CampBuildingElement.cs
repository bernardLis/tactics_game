using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingElement : VisualElement
{
    GameManager _gameManager;

    CampBuilding _campBuilding;

    VisualElement _sprite;
    Label _timeToBuild;
    VisualElement _buildingRankContainer;
    StarRankElement _buildingRankElement;

    VisualElement _buildButtonContainer;
    MyButton _buildButton;
    VisualElement _upgradeCostContainer;
    GoldElement _costGoldElement;

    TroopsLimitElement _troopsLimitElement;
    StarRankElement _betterQuestsRankElement;
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
        VisualElement upgradeContainer = new();
        upgradeContainer.style.flexDirection = FlexDirection.Row;

        if (_campBuilding.GetType().Equals(typeof(CampBuildingTroopsLimit)))
        {
            CampBuildingTroopsLimit c = (CampBuildingTroopsLimit)_campBuilding;
            _troopsLimitElement = new TroopsLimitElement($"{_gameManager.TroopsLimit.ToString()}", 24);
            upgradeContainer.Add(_troopsLimitElement);
        }

        if (_campBuilding.GetType().Equals(typeof(CampBuildingBetterQuests)))
        {
            CampBuildingBetterQuests c = (CampBuildingBetterQuests)_campBuilding;
            _betterQuestsRankElement = new(_gameManager.MaxQuestRank, 0.5f, null, 5);
            Label l = new("Max Quest Rank: ");
            upgradeContainer.style.flexDirection = FlexDirection.Column;
            upgradeContainer.style.alignItems = Align.Center;

            upgradeContainer.Add(l);
            upgradeContainer.Add(_betterQuestsRankElement);
        }

        if (_campBuilding.GetType().Equals(typeof(CampBuildingPawnshop)))
        {
            CampBuildingPawnshop c = (CampBuildingPawnshop)_campBuilding;
            string tooltipText = "Chance pawnshop visits.";
            _upgradeText = new(c.GetPawnshopVisitChance().ToString(), tooltipText);
            _upgradeText.UpdateFontSize(36);
            upgradeContainer.Add(_upgradeText);
        }


        Add(upgradeContainer);
    }

    void HandleBuildingRank()
    {
        _buildingRankElement = new(_campBuilding.UpgradeRank, 1, null, _campBuilding.UpgradeRange.y);
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
        if (_campBuilding.UpgradeRank >= _campBuilding.UpgradeRange.y)
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

    void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        if (_campBuilding.GetType().Equals(typeof(CampBuildingTroopsLimit)))
        {
            CampBuildingTroopsLimit c = (CampBuildingTroopsLimit)_campBuilding;
            int newTroopsLimit = _gameManager.TroopsLimit
                    + c.GetTroopsLimitIncreaseByRank(c.UpgradeRank + 1).LimitIncrease;
            _troopsLimitElement.UpdateCountContainer(newTroopsLimit.ToString(), Color.green);
        }

        if (_campBuilding.GetType().Equals(typeof(CampBuildingBetterQuests)))
        {
            CampBuildingBetterQuests c = (CampBuildingBetterQuests)_campBuilding;
            _betterQuestsRankElement.SetRank(_gameManager.MaxQuestRank + 1);
        }

        if (_campBuilding.GetType().Equals(typeof(CampBuildingPawnshop)))
        {
            CampBuildingPawnshop c = (CampBuildingPawnshop)_campBuilding;
            _upgradeText.UpdateText(c.GetNextUpgradePawnshopVisitChance().ToString());
            _upgradeText.UpdateTextColor(Color.green);
        }
    }

    void BuildButtonPointerLeave(PointerLeaveEvent evt) { ResetUpgradeContainer(); }

    void BuildButtonPointerUp(PointerUpEvent evt) { ResetUpgradeContainer(); ; }

    void ResetUpgradeContainer()
    {
        if (_campBuilding.GetType().Equals(typeof(CampBuildingTroopsLimit)))
        {
            CampBuildingTroopsLimit c = (CampBuildingTroopsLimit)_campBuilding;
            _troopsLimitElement.UpdateCountContainer(
                _gameManager.TroopsLimit.ToString()
                , Color.white);
        }

        if (_campBuilding.GetType().Equals(typeof(CampBuildingBetterQuests)))
        {
            CampBuildingBetterQuests c = (CampBuildingBetterQuests)_campBuilding;
            _betterQuestsRankElement.SetRank(_gameManager.MaxQuestRank);
        }

        if (_campBuilding.GetType().Equals(typeof(CampBuildingPawnshop)))
        {
            CampBuildingPawnshop c = (CampBuildingPawnshop)_campBuilding;
            _upgradeText.UpdateText(c.GetPawnshopVisitChance().ToString());
            _upgradeText.UpdateTextColor(Color.white);
        }

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

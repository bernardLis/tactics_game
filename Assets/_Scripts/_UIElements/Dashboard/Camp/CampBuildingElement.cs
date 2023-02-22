using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingElement : ElementWithTooltip
{
    protected GameManager _gameManager;

    protected CampBuilding _campBuilding;

    VisualElement _sprite;
    Label _timeToBuild;
    VisualElement _buildingRankContainer;
    StarRankElement _buildingRankElement;

    protected VisualElement _tooltipElement;

    VisualElement _buildButtonContainer;
    MyButton _buildButton;
    VisualElement _upgradeCostContainer;
    GoldElement _costGoldElement;

    protected Label _upgradeText;
    protected Label _upgradeValue;
    protected VisualElement _upgradeContainer;

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
        // Add(header);

        _sprite = new();
        _sprite.AddToClassList(_ussSprite);
        Add(_sprite);
        UpdateBuildingSprite();

        _tooltipElement = new();
        _tooltipElement.Add(header);

        HandleUpgradeReward();

        _buildingRankContainer = new();
        HandleBuildingRank();
        _tooltipElement.Add(_buildingRankContainer);

        _tooltipElement.Add(_timeToBuild);

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
        //        _upgradeContainer.style.flexDirection = FlexDirection.Row;
        _upgradeContainer.style.alignItems = Align.Center;

        _upgradeText = new();
        _upgradeContainer.Add(_upgradeText);
        _upgradeText.style.fontSize = 18;
        _upgradeText.style.whiteSpace = WhiteSpace.Normal;

        _upgradeValue = new();
        _upgradeContainer.Add(_upgradeValue);
        _upgradeValue.style.fontSize = 36;
        _upgradeValue.style.whiteSpace = WhiteSpace.Normal;


        _tooltipElement.Add(_upgradeContainer);

        Debug.Log($"handle upgrade reward");
        AddUpgrade();
        SetUpgrade();

        // Add(_upgradeContainer);
    }

    protected virtual void AddUpgrade() { }

    protected virtual void SetUpgrade() { }

    void HandleBuildingRank()
    {
        Label tt = new("Building upgrade rank.");
        _buildingRankElement = new(_campBuilding.UpgradeRank, 0.7f, tt, _campBuilding.GetMaxUpgradeRank());
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
        _gameManager.ChangeGoldValue(_campBuilding.GetUpgradeCost());
        _campBuilding.Upgrade();
        UpdateBuildButton();
        _buildingRankElement.SetRank(_campBuilding.UpgradeRank);
    }

    protected virtual void BuildButtonPointerEnter(PointerEnterEvent evt) { }

    void BuildButtonPointerLeave(PointerLeaveEvent evt) { ResetUpgradeContainer(); }

    void BuildButtonPointerUp(PointerUpEvent evt) { ResetUpgradeContainer(); }

    void ResetUpgradeContainer() { SetUpgrade(); }

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

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, _tooltipElement);
        base.DisplayTooltip();
    }

}

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
    VisualElement _buildButtonContainer;
    MyButton _buildButton;
    VisualElement _upgradeCostContainer;
    GoldElement _costGoldElement;

    TroopsLimitElement _troopsLimitElement;

    const string _ussCommonTextPrimary = "common__text-primary";

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

        Label header = new($"{_campBuilding.name}");
        Add(header);

        HandleUpgradeReward();

        _sprite = new();
        _sprite.AddToClassList(_ussSprite);
        Add(_sprite);
        UpdateBuildingSprite();

        HandleTimeToBuild();
        HandleBuildButton();
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
            _troopsLimitElement = new TroopsLimitElement($"+{c.LimitIncrease1}");
            upgradeContainer.Add(_troopsLimitElement);
        }


        Add(upgradeContainer);
    }

    void HandleTimeToBuild()
    {
        if (_campBuilding.CampBuildingState == CampBuildingState.Built || _campBuilding.CampBuildingState == CampBuildingState.Building)
            return;

        _timeToBuild = new();
        _timeToBuild.text = $"Time to build: {_campBuilding.DaysToBuild} days";
        Add(_timeToBuild);
    }

    void HandleBuildButton()
    {
        _buildButtonContainer = new();
        Add(_buildButtonContainer);
        UpdateBuildButton();
    }

    void UpdateBuildButton()
    {
        _buildButtonContainer.Clear();
        _buildButton = new(null, _ussBuildButton, Build);
        _buildButtonContainer.Add(_buildButton);
        _buildButton.SetEnabled(true);

        if (_campBuilding.CampBuildingState == CampBuildingState.NotBuilt)
        {
            _costGoldElement = new GoldElement(_campBuilding.CostToBuild);
            _buildButton.Add(_costGoldElement);
        }

        if (_campBuilding.CampBuildingState == CampBuildingState.Built)
        {
            HandleBuildButtonUpgrade();
            return;
        }

        if (_campBuilding.CampBuildingState == CampBuildingState.Building)
        {
            _buildButton.SetEnabled(false);
            _timeToBuild = new($"Days left: {_campBuilding.DaysLeftToBuild}");
            _buildButton.Add(_timeToBuild);
            return;
        }

        if (_gameManager.Gold < _campBuilding.CostToBuild)
        {
            _buildButton.SetEnabled(false);
            return;
        }

        _buildButton.UpdateButtonText("Build");
        _buildButton.SetEnabled(true);
    }

    void HandleBuildButtonUpgrade()
    {
        if (_campBuilding.UpgradeLevel < _campBuilding.UpgradeRange.y)
        {
            _buildButton.SetEnabled(false);
            _buildButton.UpdateButtonText("Upgrade");
            _costGoldElement = new GoldElement(_campBuilding.GetUpgradeCost());
            _buildButton.Add(_costGoldElement);
            _buildButton.RegisterCallback<PointerEnterEvent>(BuildButtonPointerEnter);
            _buildButton.RegisterCallback<PointerLeaveEvent>(BuildButtonPointerLeave);

            if (_gameManager.Gold < _campBuilding.GetUpgradeCost())
                _buildButton.SetEnabled(true);
            return;
        }

        _buildButtonContainer.Clear();

    }

    void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        if (_campBuilding.GetType().Equals(typeof(CampBuildingTroopsLimit)))
        {
            // TODO: get new value of troops increase
            _troopsLimitElement.UpdateCountContainer("8", Color.green);
        }


    }
    void BuildButtonPointerLeave(PointerLeaveEvent evt)
    {
        if (_campBuilding.GetType().Equals(typeof(CampBuildingTroopsLimit)))
        {
            // TODO: get current value of troops increase
            _troopsLimitElement.UpdateCountContainer("6", Color.white);
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

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.CampBuilding, null, null, null, _campBuilding.Id);
        _gameManager.AddNewReport(r);
    }
}

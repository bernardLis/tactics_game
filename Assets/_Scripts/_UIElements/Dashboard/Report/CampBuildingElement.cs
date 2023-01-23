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
        HandleUpgradeCost();
        HandleBuildButton();
    }

    void OnDayPassed(int day) { UpdateTimeToBuild(); }

    void OnGoldChanged(int gold) { UpdateBuildButton(); }

    void OnCampBuildingStateChanged(CampBuildingState newState)
    {
        if (newState == CampBuildingState.Finished)
            BuildingFinished();
    }

    void UpdateBuildingSprite()
    {
        _sprite.style.backgroundImage = new StyleBackground(_campBuilding.OutlineSprite);

        if (_campBuilding.CampBuildingState == CampBuildingState.Finished)
            _sprite.style.backgroundImage = new StyleBackground(_campBuilding.BuiltSprite);
    }

    void HandleUpgradeCost()
    {
        _upgradeCostContainer = new();
        if (_campBuilding.CampBuildingState != CampBuildingState.Pending)
            return;
        _upgradeCostContainer.AddToClassList(_ussUpgradeCostContainer);

        _upgradeCostContainer.Add(new Label("Cost: "));
        _costGoldElement = new GoldElement(_campBuilding.CostToBuild);
        _upgradeCostContainer.Add(_costGoldElement);
        Add(_upgradeCostContainer);
    }

    void HandleUpgradeReward()
    {
        VisualElement upgradeContainer = new();
        upgradeContainer.style.flexDirection = FlexDirection.Row;

        if (_campBuilding.GetType().Equals(typeof(CampBuildingTroopsLimit)))
        {
            CampBuildingTroopsLimit c = (CampBuildingTroopsLimit)_campBuilding;
            upgradeContainer.Add(new TroopsLimitElement($"+{c.LimitIncrease}"));
        }

        
        Add(upgradeContainer);
    }

    void HandleTimeToBuild()
    {
        _timeToBuild = new();
        Add(_timeToBuild);
        UpdateTimeToBuild();
    }

    void UpdateTimeToBuild()
    {
        if (_campBuilding.CampBuildingState == CampBuildingState.Finished)
        {
            _timeToBuild.text = "";
            return;
        }

        if (_campBuilding.CampBuildingState == CampBuildingState.Started)
        {
            _timeToBuild.text = $"Finished in: {_campBuilding.DaysLeftToBuild} days";
            return;
        }

        _timeToBuild.text = $"Time to build: {_campBuilding.DaysToBuild} days";
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

        _buildButton = new("Build", _ussBuildButton, Build);
        _buildButtonContainer.Add(_buildButton);
        _buildButton.SetEnabled(false);

        if (_campBuilding.CampBuildingState == CampBuildingState.Finished)
        {
            _buildButtonContainer.Clear();
            return;
        }
        if (_campBuilding.CampBuildingState == CampBuildingState.Started)
        {
            _buildButton.UpdateButtonText("Already building");
            return;
        }
        if (_gameManager.Gold < _campBuilding.CostToBuild)
        {
            _buildButton.UpdateButtonText("Insufficient funds");
            return;
        }

        _buildButton.UpdateButtonText("Build");
        _buildButton.SetEnabled(true);
    }

    void Build()
    {
        _campBuilding.StartBuilding();
        _gameManager.ChangeGoldValue(-_campBuilding.CostToBuild);
        UpdateBuildButton();
        UpdateTimeToBuild();
        _costGoldElement.ChangeAmount(0);
    }

    void BuildingFinished()
    {
        UpdateBuildingSprite();
        UpdateBuildButton();
        HandleUpgradeCost();

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.CampBuilding, null, null, null, _campBuilding.Id);
        _gameManager.AddNewReport(r);
    }
}

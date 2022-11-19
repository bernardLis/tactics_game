using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingVisualElement : VisualElement
{
    GameManager _gameManager;

    CampBuilding _campBuilding;

    VisualElement _sprite;
    Label _timeToBuild;
    MyButton _buildButton;
    VisualElement _upgradeCostContainer;
    GoldElement _costGoldElement;

    public CampBuildingVisualElement(CampBuilding campBuilding)
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
        _gameManager.OnGoldChanged += OnGoldChanged;

        _campBuilding = campBuilding;
        AddToClassList("campBuildingVisualElement");
        AddToClassList("textPrimary");

        Label header = new($"{_campBuilding.name}");
        Add(header);

        HandleUpgradeReward();

        _sprite = new();
        _sprite.AddToClassList("campBuildingSprite");
        Add(_sprite);
        UpdateBuildingSprite();

        HandleTimeToBuild();
        HandleUpgradeCost();
        HandleBuildButton();
    }

    void OnDayPassed(int day)
    {
        UpdateTimeToBuild();
        UpdateBuildingSprite();

        if (_campBuilding.IsBuilt)
            BuildingFinished();
    }

    void OnGoldChanged(int gold)
    {
        UpdateBuildButton();
    }

    void UpdateBuildingSprite()
    {
        _sprite.style.backgroundImage = _campBuilding.IsBuilt ? new StyleBackground(_campBuilding.BuiltSprite) : new StyleBackground(_campBuilding.OutlineSprite);
    }

    void HandleUpgradeCost()
    {
        if (_campBuilding.IsBuilt)
            return;

        _upgradeCostContainer = new();
        _upgradeCostContainer.style.flexDirection = FlexDirection.Row;
        _upgradeCostContainer.style.alignItems = Align.Center;

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
            upgradeContainer.Add(new TroopsLimitVisualElement(false, $"+{c.LimitIncrease}"));
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
        if (_campBuilding.IsBuilt)
        {
            _timeToBuild.text = "";
            return;
        }

        if (_campBuilding.IsBuildingStarted)
            _timeToBuild.text = $"Finished in: {_campBuilding.DaysLeftToBuild} days";
        else
            _timeToBuild.text = $"Time to build: {_campBuilding.DaysToBuild} days";
    }

    void HandleBuildButton()
    {
        _buildButton = new("Build", "", Build);
        Add(_buildButton);
        UpdateBuildButton();
    }

    void UpdateBuildButton()
    {
        if (_buildButton == null)
            return;

        _buildButton.SetEnabled(false);

        if (_campBuilding.IsBuilt)
        {
            Remove(_buildButton);
            return;
        }
        if (_campBuilding.IsBuildingStarted)
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
        _gameManager.OnGoldChanged -= OnGoldChanged;
    }

    void BuildingFinished()
    {
        if (_upgradeCostContainer != null)
            Remove(_upgradeCostContainer);
        if (_buildButton != null)
            Remove(_buildButton);

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.CampBuilding, null, null, null, _campBuilding.Id);
        _gameManager.AddNewReport(r);

        _gameManager.OnDayPassed -= OnDayPassed;

    }
}

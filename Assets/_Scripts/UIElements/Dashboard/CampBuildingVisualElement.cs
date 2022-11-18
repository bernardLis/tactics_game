using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingVisualElement : VisualElement
{
    GameManager _gameManager;

    CampBuilding _campBuilding;

    VisualElement _sprite;
    Label _timeToBuilt;
    MyButton _buildButton;

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

        _sprite = new();
        _sprite.AddToClassList("campBuildingSprite");
        UpdateBuildingSprite();

        HandleUpgradeCost();
        HandleUpgradeReward();
        HandleTimeToBuild();
        HandleBuildButton();
    }

    void OnDayPassed(int day)
    {
        HandleTimeToBuild();
        UpdateBuildingSprite();
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

        VisualElement upgradeCostContainer = new();
        upgradeCostContainer.style.flexDirection = FlexDirection.Row;
        upgradeCostContainer.Add(new Label("Cost: "));
        upgradeCostContainer.Add(new GoldElement(_campBuilding.CostToBuild));
        Add(upgradeCostContainer);
    }

    void HandleUpgradeReward()
    {
        Debug.Log($"_campBuilding.GetType() {_campBuilding.GetType()}");
        Debug.Log($"type check {_campBuilding.GetType().Equals(typeof(CampBuildingTroopsLimit))}");
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
        _timeToBuilt = new($"Time to build: {_campBuilding.DaysLeftToBuild} days");
    }

    void HandleBuildButton()
    {
        _buildButton = new("Build", "", Build);
        UpdateBuildButton();
    }

    void UpdateBuildButton()
    {
        _buildButton.SetEnabled(false);

        if (_gameManager.Gold < _campBuilding.CostToBuild)
        {
            _buildButton.text = "Insufficient funds";
            return;
        }
        if (_campBuilding.DayStartedBuilding != 0)
        {
            _buildButton.text = "Already building";
            return;
        }

        _buildButton.text = "Build";
        _buildButton.SetEnabled(true);
    }

    void Build()
    {
        _campBuilding.StartBuilding();
        _gameManager.ChangeGoldValue(-_campBuilding.CostToBuild);
        UpdateBuildButton();
    }
}

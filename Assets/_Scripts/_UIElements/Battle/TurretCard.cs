using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TurretCard : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "turret-card__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    VisualElement _leftPanel;
    VisualElement _middlePanel;
    VisualElement _rightPanel;

    Label _nameLabel;
    Label _rangeLabel;
    Label _powerLabel;
    Label _rateLabel;

    PurchaseButton _upgradeButton;

    public Turret Turret { get; private set; }

    public event Action OnShowTurretUpgrade;
    public event Action OnHideTurretUpgrade;
    public TurretCard(Turret turret)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.TurretCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        Turret = turret;

        _leftPanel = new();
        _middlePanel = new();
        _rightPanel = new();
        Add(_leftPanel);
        Add(_middlePanel);
        Add(_rightPanel);

        PopulateLeftPanel();
        PopulateMiddlePanel();
        PopulateRightPanel();
    }

    void PopulateLeftPanel()
    {
        _nameLabel = new();
        _leftPanel.Add(_nameLabel);

        TurretIcon turretIcon = new(Turret, blockTooltip: true);
        _leftPanel.Add(turretIcon);
    }

    void PopulateMiddlePanel()
    {
        _rangeLabel = new();
        _powerLabel = new();
        _rateLabel = new();

        _middlePanel.Add(_rangeLabel);
        _middlePanel.Add(_powerLabel);
        _middlePanel.Add(_rateLabel);

        UpdateTurretInfo();
    }

    void UpdateTurretInfo()
    {
        _nameLabel.text = Helpers.ParseScriptableObjectName(Turret.name)
                 + " " + Turret.CurrentTurretUpgradeIndex;
        _rangeLabel.text = "Range: " + Turret.GetCurrentUpgrade().Range;
        _powerLabel.text = "Power: " + Turret.GetCurrentUpgrade().Power;
        _rateLabel.text = "Rate of fire: " + Turret.GetCurrentUpgrade().RateOfFire;

        _nameLabel.style.color = Color.white;
        _rangeLabel.style.color = Color.white;
        _powerLabel.style.color = Color.white;
        _rateLabel.style.color = Color.white;
    }

    void PopulateRightPanel()
    {
        if (Turret.GetNextUpgrade() == null)
        {
            _upgradeButton = new(0, isPurchased: true);
            _rightPanel.Add(_upgradeButton);
            return;
        }

        _upgradeButton = new(Turret.GetNextUpgrade().Cost, isInfinite: true, callback: Upgrade);
        _rightPanel.Add(_upgradeButton);

        _upgradeButton.RegisterCallback<PointerEnterEvent>(OnUpgradeButtonPointerEnter);
        _upgradeButton.RegisterCallback<PointerLeaveEvent>(OnUpgradeButtonPointerLeave);
    }

    void Upgrade()
    {
        if (Turret.GetNextNextUpgrade() == null)
            _upgradeButton.NoMoreInfinity();

        _upgradeButton.UpdateCost(Turret.GetNextUpgrade().Cost);

        Turret.PurchaseUpgrade();
        UpdateTurretInfo();
    }

    void OnUpgradeButtonPointerEnter(PointerEnterEvent e)
    {
        TurretUpgrade currentUpgrade = Turret.GetCurrentUpgrade();
        TurretUpgrade nextUpgrade = Turret.GetNextUpgrade();
        if (nextUpgrade == null) return;

        _nameLabel.text = Helpers.ParseScriptableObjectName(Turret.name)
                 + " " + (Turret.CurrentTurretUpgradeIndex + 1);
        _rangeLabel.text = "Range: " + nextUpgrade.Range;
        _powerLabel.text = "Power: " + nextUpgrade.Power;
        _rateLabel.text = "Rate of fire: " + nextUpgrade.RateOfFire;

        _nameLabel.style.color = Color.green;

        if (currentUpgrade.Range < nextUpgrade.Range)
            _rangeLabel.style.color = Color.green;
        if (currentUpgrade.Power < nextUpgrade.Power)
            _powerLabel.style.color = Color.green;
        if (currentUpgrade.RateOfFire > nextUpgrade.RateOfFire)
            _rateLabel.style.color = Color.green;

        OnShowTurretUpgrade?.Invoke();
    }

    void OnUpgradeButtonPointerLeave(PointerLeaveEvent e)
    {
        UpdateTurretInfo();
        OnHideTurretUpgrade?.Invoke();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class StoreyTurretElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "storey-turret__";
    const string _ussMain = _ussClassName + "main";
    const string _ussContent = _ussClassName + "content";
    const string _ussTitle = _ussClassName + "title";

    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTurretsManager _battleTurretsManager;

    StoreyTurret _storey;

    VisualElement _content;
    Label _statsLabel;
    Label _killCountLabel;
    Turret _turret;

    StoreyUpgradeTreeElement _turretUpgradeTreeElement;
    StoreyUpgradeElement _specialUpgrade;

    public event Action OnClosed;
    public StoreyTurretElement(StoreyTurret storey)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StoreyTurretStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _battleManager = BattleManager.Instance;
        _battleTurretsManager = _battleManager.GetComponent<BattleTurretsManager>();
        _turret = _battleTurretsManager.GetTurret(storey.Element);

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        _storey = storey;

        _content = new();
        _content.AddToClassList(_ussContent);
        Add(_content);

        Label title = new();
        title.AddToClassList(_ussTitle);
        title.text = $"{_storey.Element.ElementName} turret.";
        _content.Add(title);

        _statsLabel = new();
        _statsLabel.AddToClassList(_ussTitle);
        _content.Add(_statsLabel);
        UpdateStats();

        _killCountLabel = new();
        _killCountLabel.AddToClassList(_ussTitle);
        _content.Add(_killCountLabel);
        _killCountLabel.text = $"Kill count: {_turret.KillCount}";

        _turretUpgradeTreeElement = new(_storey.TurretUpgradeTree);
        _turretUpgradeTreeElement.OnUpgradePurchased += TurretUpgradePurchased;
        _content.Add(_turretUpgradeTreeElement);
        AddStatComparisonOnUpgradeHover();

        _specialUpgrade = new(_storey.SpecialUpgrade);
        _specialUpgrade.OnPurchased += SpecialUpgradePurchased;
        _content.Add(_specialUpgrade);

        ContinueButton continueButton = new ContinueButton(callback: Close);
        _content.Add(continueButton);
    }

    void UpdateStats()
    {
        _statsLabel.text = $"Range: {_turret.GetCurrentUpgrade().Range}, Power: {_turret.GetCurrentUpgrade().Power}, Rate of Fire: {_turret.GetCurrentUpgrade().RateOfFire}";
    }

    void AddStatComparisonOnUpgradeHover()
    {
        foreach (StoreyUpgradeElement el in _turretUpgradeTreeElement.UpgradeElements)
        {
            el.RegisterCallback<PointerEnterEvent>(ev =>
            {
                if (el.StoreyUpgrade == null)
                    return;
                int index = _turretUpgradeTreeElement.UpgradeElements.IndexOf(el);
                string r = $"Range: {_turret.TurretUpgrades[index].Range}";
                string p = $", Power: {_turret.TurretUpgrades[index].Power}";
                string f = $", Rate of Fire: {_turret.TurretUpgrades[index].RateOfFire}";
                _statsLabel.text = r + p + f;
            });

            el.RegisterCallback<PointerLeaveEvent>(ev =>
            {
                UpdateStats();
            });
        }
    }

    void TurretUpgradePurchased(StoreyUpgrade storeyUpgrade)
    {
        _battleTurretsManager.UpgradeTurret(_storey.Element);
        UpdateStats();
    }

    void SpecialUpgradePurchased(StoreyUpgrade storeyUpgrade)
    {
        _battleTurretsManager.SpecialUpgradePurchased(_storey.Element);
    }

    void Close()
    {
        DOTween.To(x => style.opacity = x, style.opacity.value, 0, 0.5f).SetUpdate(true);
        DOTween.To(x => _content.style.opacity = x, 1, 0, 0.5f)
            .SetUpdate(true)
            .OnComplete(() =>
                {
                    OnClosed?.Invoke();
                    RemoveFromHierarchy();
                });
    }
}

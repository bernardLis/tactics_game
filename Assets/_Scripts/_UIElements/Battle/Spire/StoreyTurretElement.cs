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
    Label _title;
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

        _title = new();
        _title.text = $"{storey.Element.ElementName} turret. Range: {_turret.GetCurrentUpgrade().Range}, power: {_turret.GetCurrentUpgrade().Power}, rate of fire: {_turret.GetCurrentUpgrade().RateOfFire}";
        _title.AddToClassList(_ussTitle);
        _content.Add(_title);

        _turretUpgradeTreeElement = new(_storey.TurretUpgradeTree);
        _turretUpgradeTreeElement.OnUpgradePurchased += TurretUpgradePurchased;
        _content.Add(_turretUpgradeTreeElement);

        _specialUpgrade = new(_storey.SpecialUpgrade);
        _specialUpgrade.OnPurchased += SpecialUpgradePurchased;
        _content.Add(_specialUpgrade);

        ContinueButton continueButton = new ContinueButton(callback: Close);
        _content.Add(continueButton);
    }

    void TurretUpgradePurchased(StoreyUpgrade storeyUpgrade)
    {
        _battleTurretsManager.UpgradeTurret(_storey.Element);
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

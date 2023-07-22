using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class StoreyTurretsElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "storey-turrets__";
    const string _ussMain = _ussClassName + "main";
    const string _ussContent = _ussClassName + "content";

    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTurretsManager _battleTurretsManager;

    StoreyTurrets _storey;

    VisualElement _content;

    StoreyUpgradeTreeElement _earthTurretUpgradeTreeElement;
    StoreyUpgradeTreeElement _fireTurretUpgradeTreeElement;
    StoreyUpgradeTreeElement _waterTurretUpgradeTreeElement;
    StoreyUpgradeTreeElement _windTurretUpgradeTreeElement;

    public event Action OnClosed;
    public StoreyTurretsElement(StoreyTurrets storey)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StoreyTurretsStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _battleManager = BattleManager.Instance;
        _battleTurretsManager = _battleManager.GetComponent<BattleTurretsManager>();

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        _storey = storey;

        _content = new();
        _content.AddToClassList(_ussContent);
        Add(_content);

        _earthTurretUpgradeTreeElement = new(_storey.EarthTurretUpgradeTree);
        _earthTurretUpgradeTreeElement.OnUpgradePurchased += EarthTurretUpgradePurchased;
        _content.Add(_earthTurretUpgradeTreeElement);

        _fireTurretUpgradeTreeElement = new(_storey.FireTurretUpgradeTree);
        _fireTurretUpgradeTreeElement.OnUpgradePurchased += FireTurretUpgradePurchased;
        _content.Add(_fireTurretUpgradeTreeElement);

        _windTurretUpgradeTreeElement = new(_storey.WindTurretUpgradeTree);
        _windTurretUpgradeTreeElement.OnUpgradePurchased += WindTurretUpgradePurchased;
        _content.Add(_windTurretUpgradeTreeElement);

        _waterTurretUpgradeTreeElement = new(_storey.WaterTurretUpgradeTree);
        _waterTurretUpgradeTreeElement.OnUpgradePurchased += WaterTurretUpgradePurchased;
        _content.Add(_waterTurretUpgradeTreeElement);

        ContinueButton continueButton = new ContinueButton(callback: Close);
        _content.Add(continueButton);
    }

    void EarthTurretUpgradePurchased(StoreyUpgrade storeyUpgrade)
    {
        if (storeyUpgrade.Value == 0)
        {
            _battleTurretsManager.InstantiateEarthTurret();
            return;
        }
        _battleTurretsManager.UpgradeEarthTurret();
    }

    void FireTurretUpgradePurchased(StoreyUpgrade storeyUpgrade)
    {
        if (storeyUpgrade.Value == 0)
        {
            _battleTurretsManager.InstantiateFireTurret();
            return;
        }
        _battleTurretsManager.UpgradeFireTurret();
    }

    void WaterTurretUpgradePurchased(StoreyUpgrade storeyUpgrade)
    {
        if(storeyUpgrade.Value == 0)
        {
            _battleTurretsManager.InstantiateWaterTurret();
            return;
        }
        _battleTurretsManager.UpgradeWaterTurret();
    }

    void WindTurretUpgradePurchased(StoreyUpgrade storeyUpgrade)
    {
        if (storeyUpgrade.Value == 0)
        {
            _battleTurretsManager.InstantiateWindTurret();
            return;
        }
        _battleTurretsManager.UpgradeWindTurret();
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

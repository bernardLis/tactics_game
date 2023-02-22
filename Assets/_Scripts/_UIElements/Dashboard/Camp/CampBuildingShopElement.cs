using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingShopElement : CampBuildingElement
{
    public CampBuildingShopElement(CampBuildingShop campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        _upgradeText.text = _campBuilding.TooltipText.Value;
        SetUpgrade();
    }

    protected override void SetUpgrade()
    {
        CampBuildingShop c = (CampBuildingShop)_campBuilding;
        CampUpgradeShop upgrade = c.GetUpgradeByRank(c.UpgradeRank);
        string labelText = $"{upgrade.UncommonItemChance}, {upgrade.RareItemChance}, {upgrade.EpicItemChance}";
        _upgradeValue.text = $"{labelText}";
        _upgradeValue.style.color = Color.white;
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingShop c = (CampBuildingShop)_campBuilding;
        CampUpgradeShop upgrade = c.GetUpgradeByRank(c.UpgradeRank + 1);
        string labelText = $"{upgrade.UncommonItemChance}, {upgrade.RareItemChance}, {upgrade.EpicItemChance}";
        _upgradeValue.text = $"{labelText}";
        _upgradeValue.style.color = Color.green;
    }

}

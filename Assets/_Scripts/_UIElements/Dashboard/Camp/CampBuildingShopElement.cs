using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CampBuildingShopElement : CampBuildingElement
{

    TextWithTooltip _upgradeText;

    public CampBuildingShopElement(CampBuildingShop campBuilding) : base(campBuilding)
    {
    }

    protected override void AddUpgrade()
    {
        string tooltipText = "Chance for uncommon, rare and epic item in the shop.";
        _upgradeText = new("TXT", tooltipText);
        SetUpgrade();
        _upgradeText.UpdateFontSize(18);
        _upgradeContainer.Add(_upgradeText);
    }

    protected override void SetUpgrade()
    {
        CampBuildingShop c = (CampBuildingShop)_campBuilding;
        CampUpgradeShop upgrade = c.GetUpgradeByRank(c.UpgradeRank);
        string labelText = $"{upgrade.UncommonItemChance}, {upgrade.RareItemChance}, {upgrade.EpicItemChance}";
        _upgradeText.UpdateText(labelText);
        _upgradeText.UpdateTextColor(Color.white);
    }

    protected override void BuildButtonPointerEnter(PointerEnterEvent evt)
    {
        CampBuildingShop c = (CampBuildingShop)_campBuilding;
        CampUpgradeShop upgrade = c.GetUpgradeByRank(c.UpgradeRank + 1);
        string labelText = $"{upgrade.UncommonItemChance}, {upgrade.RareItemChance}, {upgrade.EpicItemChance}";
        _upgradeText.UpdateText(labelText);
        _upgradeText.UpdateTextColor(Color.green);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TurretCard : EntityCard
{
    PurchaseButton _upgradeButton;

    public Turret Turret { get; private set; }

    public TurretCard(Turret turret) : base(turret)
    {
        Turret = turret;
        PopulateCard();
        PopulateRightPanel();
    }


    void PopulateRightPanel()
    {
        _topRightContainer.Add(new Label("<b>Upgrade</b>"));
        _upgradeButton = new(Turret.UpgradeCost, isInfinite: true, callback: Upgrade);
        _topRightContainer.Add(_upgradeButton);
    }

    void Upgrade()
    {
        _upgradeButton.UpdateCost(Turret.UpgradeCost);

        Turret.PurchaseUpgrade();
    }
}

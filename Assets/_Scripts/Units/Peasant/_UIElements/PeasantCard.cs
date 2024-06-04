using System.Collections.Generic;
using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units.Peasant
{
    public class PeasantCard : UnitCard
    {
        const string _ussCommonButton = "common__button";

        readonly Peasant _peasant;

        public PeasantCard(Peasant peasant) : base(peasant)
        {
            _peasant = peasant;
            if (FightManager.IsFightActive) return;
            if (_peasant.CurrentHealth.Value <= 0) return;
            AddUpgradeButtons();
        }

        void AddUpgradeButtons()
        {
            List<Nature> availableNatures = new();
            BattleManager.Instance.Battle.Barracks.UnlockableNatures.ForEach(un =>
            {
                if (un.IsUnlocked) availableNatures.Add(un.Nature);
            });

            foreach (Nature n in availableNatures)
            {
                // HERE: balance price
                PurchaseButton b = new("", _ussCommonButton, () => Upgrade(n), 100);
                b.Add(new NatureElement(n));
                BottomContainer.Add(b);
            }
        }

        void Upgrade(Nature n)
        {
            if (GameManager.Gold < 100)
            {
                Helpers.DisplayTextOnElement(BattleManager.Instance.Root, this, "Not enough gold", Color.red);
                return;
            }

            GameManager.ChangeGoldValue(-100);
            _peasant.Upgrade(n);
        }
    }
}
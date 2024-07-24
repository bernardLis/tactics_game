using System.Collections.Generic;
using Lis.Arena;
using Lis.Arena.Fight;
using Lis.Core;

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
            FightManager.Instance.Campaign.Barracks.UnlockableNatures.ForEach(un =>
            {
                if (un.CurrentLevel > 0) availableNatures.Add(un.Nature);
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
            _peasant.Upgrade(n);
        }
    }
}
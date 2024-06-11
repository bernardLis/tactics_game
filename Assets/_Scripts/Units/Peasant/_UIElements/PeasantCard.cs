using System.Collections.Generic;
using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Core;

namespace Lis.Units.Peasant
{
    public class PeasantCard : UnitCard
    {
        private const string _ussCommonButton = "common__button";

        private readonly Peasant _peasant;

        public PeasantCard(Peasant peasant) : base(peasant)
        {
            _peasant = peasant;
            if (FightManager.IsFightActive) return;
            if (_peasant.CurrentHealth.Value <= 0) return;
            AddUpgradeButtons();
        }

        private void AddUpgradeButtons()
        {
            List<Nature> availableNatures = new();
            BattleManager.Instance.Battle.Barracks.UnlockableNatures.ForEach(un =>
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

        private void Upgrade(Nature n)
        {
            _peasant.Upgrade(n);
        }
    }
}
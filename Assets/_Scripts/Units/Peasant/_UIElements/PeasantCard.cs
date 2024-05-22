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
        readonly UnitDatabase _unitDatabase;

        readonly Peasant _peasant;

        public PeasantCard(Peasant peasant) : base(peasant)
        {
            _unitDatabase = GameManager.UnitDatabase;
            _peasant = peasant;
            if (FightManager.IsFightActive) return;
            if (_peasant.CurrentHealth.Value <= 0) return;
            AddUpgradeButtons();
        }

        void AddUpgradeButtons()
        {
            Nature earth = _unitDatabase.GetNatureByName(NatureName.Earth);
            Nature fire = _unitDatabase.GetNatureByName(NatureName.Fire);
            Nature water = _unitDatabase.GetNatureByName(NatureName.Water);
            Nature wind = _unitDatabase.GetNatureByName(NatureName.Wind);

            List<Nature> natures = new List<Nature> { earth, fire, water, wind };

            foreach (Nature n in natures)
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
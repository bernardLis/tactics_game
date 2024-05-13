﻿using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units.Pawn
{
    public class PawnCard : UnitCard
    {
        const string _ussCommonButton = "common__button";

        readonly FightManager _fightManager;

        readonly PurchaseButton _upgradeButton;

        readonly Pawn _pawn;

        public PawnCard(Pawn pawn) : base(pawn)
        {
            _pawn = pawn;

            _fightManager = FightManager.Instance;
            if (FightManager.IsFightActive) return;
            if (_pawn.GetNextUpgrade() == null) return;
            _upgradeButton = new("", _ussCommonButton, Upgrade, pawn.GetNextUpgrade().Price);
            BottomContainer.Add(_upgradeButton);
            _fightManager.OnFightStarted += () => _upgradeButton.RemoveFromHierarchy();
        }

        void Upgrade()
        {
            if (_pawn.GetNextUpgrade().Price > GameManager.Gold)
            {
                Helpers.DisplayTextOnElement(BattleManager.Instance.Root, _upgradeButton, "Not enough gold!",
                    Color.red);
                return;
            }

            GameManager.ChangeGoldValue(-_pawn.GetCurrentUpgrade().Price);
            _pawn.Upgrade();
            PopulateCard();
            _upgradeButton.SetEnabled(false);

            if (_pawn.GetNextUpgrade() == null)
            {
                _upgradeButton.RemoveFromHierarchy();
                return;
            }

            _upgradeButton.ChangePrice(_pawn.GetNextUpgrade().Price);
            schedule.Execute(() => _upgradeButton.SetEnabled(true)).StartingIn(3500); // upgrade effect duration
        }
    }
}
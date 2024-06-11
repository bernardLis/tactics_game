using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Core;

namespace Lis.Units.Pawn
{
    public class PawnCard : UnitCard
    {
        private const string _ussCommonButton = "common__button";

        private readonly FightManager _fightManager;

        private readonly Pawn _pawn;

        private readonly PurchaseButton _upgradeButton;

        public PawnCard(Pawn pawn) : base(pawn)
        {
            _pawn = pawn;

            _fightManager = FightManager.Instance;
            if (FightManager.IsFightActive) return;
            if (_pawn.GetNextUpgrade() == null) return;
            if (_pawn.CurrentHealth.Value <= 0) return;
            _upgradeButton = new("", _ussCommonButton, Upgrade, pawn.GetNextUpgrade().Price);
            BottomContainer.Add(_upgradeButton);
            _fightManager.OnFightStarted += () => _upgradeButton.RemoveFromHierarchy();
        }

        private void Upgrade()
        {
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
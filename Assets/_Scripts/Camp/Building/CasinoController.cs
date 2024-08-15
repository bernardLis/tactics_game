using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class CasinoController : BuildingController, IInteractable
    {
        [SerializeField] SpinWheelController _spinWheelController;

        Casino _casino;
        public new string InteractionPrompt => "Access Casino";

        protected override void Initialize()
        {
            Building = GameManager.Campaign.Casino;
            _casino = (Casino)Building;

            _spinWheelController.OnSpinCompleted += SpinCompleted;
            base.Initialize();
        }

        public override bool Interact(Interactor interactor)
        {
            CasinoScreen casinoScreen = new();
            casinoScreen.InitializeCasino(_casino);

            casinoScreen.OnHide += () =>
            {
                if (_casino.IsBetPlaced)
                    _spinWheelController.Spin();
            };
            return true;
        }

        void SpinCompleted(float angle)
        {
            // seems like a bad code, but meh
            if (angle is > 0 and < 180)
            {
                if (_casino.IsGreenBet)
                    Won();
                else
                    Lost();
            }
            else
            {
                if (_casino.IsGreenBet)
                    Lost();
                else
                    Won();
            }
        }

        void Won()
        {
            GameManager.ChangeGoldValue(_casino.BetAmount * 2);
            CampConsoleManager.ShowMessage($"You won! +{_casino.BetAmount * 2} Gold");
            HeroCampController.Instance.DisplayFloatingText($"You won! +{_casino.BetAmount * 2} Gold", Color.black);
        }

        void Lost()
        {
            CampConsoleManager.ShowMessage($"You lost {_casino.BetAmount} Gold");
            HeroCampController.Instance.DisplayFloatingText($"You lost!", Color.black);
        }
    }
}
using System;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class CasinoController : BuildingController, IInteractable
    {
        [SerializeField] SpinWheelController _spinWheelController;
        [SerializeField] CasinoSkeletonController _casinoSkeletonController;
        Casino _casino;
        public new string InteractionPrompt => "Access Casino";

        bool _isSpinning;

        public event Action OnSpin;
        public event Action<bool> OnSpinCompleted;

        protected override void Initialize()
        {
            Building = GameManager.Campaign.Casino;
            _casino = (Casino)Building;

            _spinWheelController.OnSpinCompleted += SpinCompleted;
            _casinoSkeletonController.Initialize(this);
            base.Initialize();
        }

        void OnDisable()
        {
            _spinWheelController.OnSpinCompleted -= SpinCompleted;
        }

        void OnDestroy()
        {
            _spinWheelController.OnSpinCompleted -= SpinCompleted;
        }

        public override bool CanInteract()
        {
            if (_isSpinning) return false;
            return base.CanInteract();
        }


        public override bool Interact(Interactor interactor)
        {
            CasinoScreen casinoScreen = new();
            casinoScreen.InitializeCasino(_casino);

            casinoScreen.OnHide += () =>
            {
                if (_casino.IsBetPlaced)
                {
                    _isSpinning = true;
                    _spinWheelController.Spin();
                    OnSpin?.Invoke();
                }
            };
            return true;
        }

        void SpinCompleted(float angle)
        {
            if (_spinWheelController == null) return;

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

            _isSpinning = false;
            LookForInteractor();
        }

        void Won()
        {
            GameManager.ChangeGoldValue(_casino.BetAmount * 2);
            CampConsoleManager.ShowMessage($"You won! +{_casino.BetAmount * 2} Gold");
            HeroCampController.Instance.DisplayFloatingText($"You won! +{_casino.BetAmount * 2} Gold", Color.black);
            OnSpinCompleted?.Invoke(true);
        }

        void Lost()
        {
            CampConsoleManager.ShowMessage($"You lost {_casino.BetAmount} Gold");
            HeroCampController.Instance.DisplayFloatingText($"You lost!", Color.black);
            OnSpinCompleted?.Invoke(false);
        }
    }
}
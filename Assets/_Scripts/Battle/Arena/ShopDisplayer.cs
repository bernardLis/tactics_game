using Lis.Battle.Fight;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class ShopDisplayer : ArenaInteractable, IInteractable
    {
        BattleManager _battleManager;
        public new string InteractionPrompt => "Press F To Shop!";
        Shop _shop;

        protected override void Start()
        {
            base.Start();

            _battleManager = BattleManager.Instance;
            _battleManager.GetComponent<BattleInitializer>().OnBattleInitialized += OnBattleInitialized;
        }

        void OnBattleInitialized()
        {
            _shop = _battleManager.Battle.Shop;
            OnFightEnded();
        }

        protected override void OnFightEnded()
        {
            InteractionAvailableEffect.SetActive(true);
            IsInteractionAvailable = true;
            _shop.ShouldReset = true;
        }

        protected override void OnFightStarted()
        {
            InteractionAvailableEffect.SetActive(false);
            HideTooltip();
            IsInteractionAvailable = false;
        }


        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight instead of shopping!");
                return false;
            }

            ShopScreen shopScreen = new ShopScreen();
            shopScreen.InitializeShop(_shop);
            // HERE: testing  OnFightStarted();
            return true;
        }
    }
}
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class ShopDisplayer : ArenaInteractable, IInteractable
    {
        public new string InteractionPrompt => "Press F To Shop!";
        Shop _shop;

        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            _shop = BattleManager.Battle.Shop;
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
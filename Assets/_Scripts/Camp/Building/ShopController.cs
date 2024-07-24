using Lis.Arena.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class ShopController : BuildingController, IInteractable
    {
        Shop _shop;
        public new string InteractionPrompt => "Shop";

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight instead of shopping!");
                return false;
            }

            ShopScreen shopScreen = new();
            shopScreen.InitializeShop(_shop);
            return true;
        }

        protected override void OnArenaInitialized()
        {
            base.OnArenaInitialized();
            Building = FightManager.Campaign.Shop;
            _shop = (Shop)Building;
            OnFightEnded();
            Initialize();
        }

        protected override void OnFightEnded()
        {
            if (!_shop.IsUnlocked) return;
            AllowInteraction();
            _shop.ShouldReset = true;
        }

        protected override void OnFightStarted()
        {
            if (!_shop.IsUnlocked) return;
            ForbidInteraction();
        }

        protected override void Unlock()
        {
            base.Unlock();
            OnFightEnded();
        }
    }
}
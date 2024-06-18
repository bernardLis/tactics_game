using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena.Building
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

        protected override void OnBattleInitialized()
        {
            base.OnBattleInitialized();
            Building = BattleManager.Battle.Shop;
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
    }
}
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

        protected override void Initialize()
        {
            base.Initialize();
            Building = GameManager.Campaign.Shop;
            _shop = (Shop)Building;
            AllowInteraction();
            _shop.ShouldReset = true;

        }

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
    }
}
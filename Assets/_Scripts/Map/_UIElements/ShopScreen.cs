using Lis.Core;
using Lis.Core.Utilities;
using Lis.Map.MapNodes;
using Lis.Units.Hero.Rewards;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Map
{
    public class ShopScreen : RewardScreen
    {
        MapNodeShop _shop;

        public void InitializeShop(MapNodeShop shop)
        {
            _shop = shop;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            AddContinueButton();
            SetTitle("Shop");
        }

        protected override void RerollReward()
        {
            if (Hero.RewardRerolls <= 0)
            {
                Helpers.DisplayTextOnElement(FightManager.Root, RerollButton, "Not More Rerolls!", Color.red);
                return;
            }

            Hero.RewardRerolls--;
            RerollsLeft.text = $"Rerolls left: {Hero.RewardRerolls}";
            AudioManager.CreateSound().WithSound(AudioManager.GetSound("Dice Roll")).Play();
            _shop.SelectItems();
            CreateRewardCards();

            if (Hero.RewardRerolls <= 0)
                RerollButton.SetEnabled(false);
        }

        protected override void CreateRewardCards()
        {
            AllRewardElements.Clear();
            RewardContainer.Clear();

            ParseRewardCards(_shop.GetRewards());

            AddShopItems();
        }

        void AddShopItems()
        {
            for (int i = 0; i < NumberOfRewards; i++)
            {
                ShopItemElement shopItemElement = new(AllRewardElements[i]);

                shopItemElement.style.position = Position.Absolute;
                float endLeft = LeftPositions[i];
                shopItemElement.style.left = endLeft;

                shopItemElement.style.width = RewardElementWidth;
                shopItemElement.style.height = RewardElementHeight;

                RewardContainer.Add(shopItemElement);
            }
        }

        protected override void RewardSelected(Reward reward)
        {
            // leave empty
        }
    }
}